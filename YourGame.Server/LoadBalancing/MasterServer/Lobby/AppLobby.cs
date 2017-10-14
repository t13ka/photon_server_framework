using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Photon.SocketServer.Rpc.Protocols;

using YourGame.Server.Common;
using YourGame.Server.Events;
using YourGame.Server.MasterServer.ChannelLobby;
using YourGame.Server.MasterServer.GameServer;
using YourGame.Server.Operations;
using YourGame.Server.Operations.Request;
using YourGame.Server.Operations.Response;
using YourGame.Server.ServerToServer.Events;

namespace YourGame.Server.MasterServer.Lobby
{
    using YourGame.Server.Operations.Request.GameManagement;

    using YourGame.Common;

    public class AppLobby
    {
        #region Properties

        public PoolFiber ExecutionFiber { get; }

        #endregion

        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public readonly GameApplication Application;

        public readonly string LobbyName;

        public readonly TimeSpan JoinTimeOut = TimeSpan.FromSeconds(5);

        public readonly int MaxPlayersDefault;

        internal readonly IGameList GameList;

        private readonly HashSet<PeerBase> peers = new HashSet<PeerBase>();

        private IDisposable _schedule;

        private IDisposable _checkJoinTimeoutSchedule;

        #endregion

        #region Constructors and Destructors

        public AppLobby(GameApplication application, string lobbyName, AppLobbyType lobbyType)
            : this(application, lobbyName, lobbyType, 0, TimeSpan.FromSeconds(15))
        {
        }

        public AppLobby(GameApplication application, string lobbyName, AppLobbyType lobbyType, int maxPlayersDefault,
            TimeSpan joinTimeOut)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Creating lobby: name={0}, type={1}", lobbyName, lobbyType);
            }

            Application = application;
            LobbyName = lobbyName;
            MaxPlayersDefault = maxPlayersDefault;
            JoinTimeOut = joinTimeOut;

            switch (lobbyType)
            {
                default:
                    GameList = new GameList(this);
                    break;

                case AppLobbyType.ChannelLobby:
                    GameList = new GameChannelList(this);
                    break;

                case AppLobbyType.DataBaseLobby:
                    // YET NOT USED !
                    //GameList = new SqlGameList(this);
                    break;
            }

            ExecutionFiber = new PoolFiber();
            ExecutionFiber.Start();
        }

        #endregion

        #region Public Methods

        public void EnqueueOperation(MasterClientPeer peer, OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            ExecutionFiber.Enqueue(() => ExecuteOperation(peer, operationRequest, sendParameters));
        }


        public void RemoveGame(string gameId)
        {
            ExecutionFiber.Enqueue(() => HandleRemoveGameState(gameId));
        }

        public void RemoveGameServer(IncomingGameServerPeer gameServer)
        {
            ExecutionFiber.Enqueue(() => HandleRemoveGameServer(gameServer));
        }

        public void RemovePeer(MasterClientPeer serverPeer)
        {
            ExecutionFiber.Enqueue(() => HandleRemovePeer(serverPeer));
        }

        public void UpdateGameState(UpdateGameEvent operation, IncomingGameServerPeer incomingGameServerPeer)
        {
            ExecutionFiber.Enqueue(() => HandleUpdateGameState(operation, incomingGameServerPeer));
        }

        public void JoinLobby(MasterClientPeer peer, JoinLobbyRequest joinLobbyrequest, SendParameters sendParameters)
        {
            ExecutionFiber.Enqueue(() => HandleJoinLobby(peer, joinLobbyrequest, sendParameters));
        }

        #endregion

        #region Methods

        protected virtual void ExecuteOperation(MasterClientPeer peer, OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            try
            {
                OperationResponse response;

                switch ((OperationCode)operationRequest.OperationCode)
                {
                    default:
                        response = new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode = -1,
                            DebugMessage = "Unknown operation code"
                        };
                        break;

                    case OperationCode.JoinLobby:
                        var joinLobbyRequest = new JoinLobbyRequest(peer.Protocol, operationRequest);
                        if (OperationHelper.ValidateOperation(joinLobbyRequest, Log, out response))
                        {
                            response = HandleJoinLobby(peer, joinLobbyRequest, sendParameters);
                        }
                        break;

                    case OperationCode.LeaveLobby:
                        response = HandleLeaveLobby(peer, operationRequest);
                        break;

                    case OperationCode.CreateGame:
                        response = HandleCreateGame(peer, operationRequest);
                        break;

                    case OperationCode.JoinGame:
                        response = HandleJoinGame(peer, operationRequest);
                        break;

                    case OperationCode.JoinRandomGame:
                        response = HandleJoinRandomGame(peer, operationRequest);
                        break;

                    case OperationCode.DebugGame:
                        response = HandleDebugGame(peer, operationRequest);
                        break;
                }

                if (response != null)
                {
                    peer.SendOperationResponse(response, sendParameters);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected virtual object GetCreateGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new CreateGameResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer) };
        }

        protected virtual object GetJoinGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new JoinGameResponse { Address = gameState.GetServerAddress(peer) };
        }

        protected virtual object GetJoinRandomGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new JoinRandomGameResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer) };
        }

        protected virtual DebugGameResponse GetDebugGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new DebugGameResponse
            {
                Address = gameState.GetServerAddress(peer),
                Info = gameState.ToString()
            };
        }

        protected virtual OperationResponse HandleCreateGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate the operation request
            OperationResponse response;
            var operation = new CreateGameRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, Log, out response) == false)
            {
                return response;
            }

            // special handling for game properties send by AS3/Flash (Amf3 protocol) or JSON clients
            var protocol = peer.Protocol.ProtocolType;
            if (protocol == ProtocolType.Amf3V152 || protocol == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(operation.GameProperties, null);
                // special treatment for game properties sent by AS3/Flash
            }

            // if no gameId is specified by the client generate a unique id 
            if (string.IsNullOrEmpty(operation.GameId))
            {
                operation.GameId = Guid.NewGuid().ToString();
            }

            // try to create game
            GameState gameState;
            bool gameCreated;
            if (
                !TryCreateGame(operation, operation.GameId, false, operation.GameProperties, out gameCreated,
                    out gameState, out response))
            {
                return response;
            }

            // add peer to game
            gameState.AddPeer(peer);

            ScheduleCheckJoinTimeOuts();

            // publish operation response
            var createGameResponse = GetCreateGameResponse(peer, gameState);
            return new OperationResponse(operationRequest.OperationCode, createGameResponse);
        }

        protected virtual OperationResponse HandleJoinGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate operation
            var operation = new JoinGameRequest(peer.Protocol, operationRequest);
            OperationResponse response;
            if (OperationHelper.ValidateOperation(operation, Log, out response) == false)
            {
                return response;
            }

            // special handling for game properties send by AS3/Flash (Amf3 protocol) or JSON clients
            var protocol = peer.Protocol.ProtocolType;
            if (protocol == ProtocolType.Amf3V152 || protocol == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(operation.GameProperties, null);
            }


            // try to find game by id
            GameState gameState;
            var gameCreated = false;
            if (operation.CreateIfNotExists == false)
            {
                // The client does not want to create the game if it does not exists.
                // In this case the game must have been created on the game server before it can be joined.
                if (GameList.TryGetGame(operation.GameId, out gameState) == false ||
                    gameState.IsCreatedOnGameServer == false)
                {
                    return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode,
                        ReturnCode = (int)ErrorCode.GameIdNotExists,
                        DebugMessage = "Game does not exist"
                    };
                }
            }
            else
            {
                // The client will create the game if it does not exists already.
                if (!GameList.TryGetGame(operation.GameId, out gameState))
                {
                    if (
                        !TryCreateGame(operation, operation.GameId, true, operation.GameProperties, out gameCreated,
                            out gameState, out response))
                    {
                        return response;
                    }
                }
            }

            // game properties have only be to checked if the game was not created by the client
            if (gameCreated == false)
            {
                // check if max players of the game is already reached
                if (gameState.MaxPlayer > 0 && gameState.PlayerCount >= gameState.MaxPlayer)
                {
                    return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode,
                        ReturnCode = (int)ErrorCode.GameFull,
                        DebugMessage = "Game full"
                    };
                }

                // check if the game is open
                if (gameState.IsOpen == false)
                {
                    return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode,
                        ReturnCode = (int)ErrorCode.GameClosed,
                        DebugMessage = "Game closed"
                    };
                }
            }

            // add peer to game
            gameState.AddPeer(peer);

            ScheduleCheckJoinTimeOuts();

            // publish operation response
            var joinResponse = GetJoinGameResponse(peer, gameState);
            return new OperationResponse(operationRequest.OperationCode, joinResponse);
        }

        protected virtual OperationResponse HandleJoinLobby(MasterClientPeer peer, JoinLobbyRequest operation,
            SendParameters sendParameters)
        {
            try
            {
                // special handling for game properties send by AS3/Flash (Amf3 protocol) clients
                if (peer.Protocol.ProtocolType == ProtocolType.Amf3V152 ||
                    peer.Protocol.ProtocolType == ProtocolType.Json)
                {
                    Utilities.ConvertAs3WellKnownPropertyKeys(operation.GameProperties, null);
                }

                peer.GameChannelSubscription = null;

                var subscription = GameList.AddSubscription(peer, operation.GameProperties, operation.GameListCount);
                peer.GameChannelSubscription = subscription;
                peer.SendOperationResponse(new OperationResponse(operation.OperationRequest.OperationCode),
                    sendParameters);


                // publish game list to peer after the response has been sent
                var gameList = subscription.GetGameList();
                var e = new GameListEvent { Data = gameList };
                var eventData = new EventData((byte)EventCode.GameList, e);
                peer.SendEvent(eventData, new SendParameters());

                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        protected virtual OperationResponse HandleJoinRandomGame(MasterClientPeer peer,
            OperationRequest operationRequest)
        {
            // validate the operation request
            var operation = new JoinRandomGameRequest(peer.Protocol, operationRequest);
            OperationResponse response;
            if (OperationHelper.ValidateOperation(operation, Log, out response) == false)
            {
                return response;
            }

            // special handling for game properties send by AS3/Flash (Amf3 protocol) clients
            if (peer.Protocol.ProtocolType == ProtocolType.Amf3V152 || peer.Protocol.ProtocolType == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(operation.GameProperties, null);
            }

            // try to find a match
            GameState game;
            string errorMessage;
            var result = GameList.TryGetRandomGame((JoinRandomType)operation.JoinRandomType, peer,
                operation.GameProperties, operation.QueryData, out game, out errorMessage);
            if (result != ErrorCode.Ok)
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "No match found";
                }

                response = new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (short)result,
                    DebugMessage = errorMessage
                };
                return response;
            }

            // match found, add peer to game and notify the peer
            game.AddPeer(peer);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Found match: connectionId={0}, userId={1}, gameId={2}", peer.ConnectionId, peer.UserId,
                    game.Id);
            }

            ScheduleCheckJoinTimeOuts();

            var joinResponse = GetJoinRandomGameResponse(peer, game);
            return new OperationResponse(operationRequest.OperationCode, joinResponse);
        }

        protected virtual OperationResponse HandleLeaveLobby(MasterClientPeer peer, OperationRequest operationRequest)
        {
            peer.GameChannelSubscription = null;

            if (peers.Remove(peer))
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode };
            }

            return new OperationResponse
            {
                OperationCode = operationRequest.OperationCode,
                ReturnCode = 0,
                DebugMessage = "lobby not joined"
            };
        }

        protected virtual OperationResponse HandleDebugGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            var operation = new DebugGameRequest(peer.Protocol, operationRequest);
            OperationResponse response;
            if (OperationHelper.ValidateOperation(operation, Log, out response) == false)
            {
                return response;
            }

            GameState gameState;
            if (GameList.TryGetGame(operation.GameId, out gameState) == false)
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (int)ErrorCode.GameIdNotExists,
                    DebugMessage = "Game does not exist"
                };
            }

            var debugGameResponse = GetDebugGameResponse(peer, gameState);

            Log.InfoFormat("DebugGame: {0}", debugGameResponse.Info);

            return new OperationResponse(operationRequest.OperationCode, debugGameResponse);
        }

        protected virtual void OnGameStateChanged(GameState gameState)
        {
        }

        protected virtual void OnRemovePeer(MasterClientPeer peer)
        {
        }

        private void ScheduleCheckJoinTimeOuts()
        {
            if (_checkJoinTimeoutSchedule == null)
            {
                _checkJoinTimeoutSchedule = ExecutionFiber.Schedule(CheckJoinTimeOuts,
                    (long)JoinTimeOut.TotalMilliseconds / 2);
            }
        }

        private void CheckJoinTimeOuts()
        {
            try
            {
                _checkJoinTimeoutSchedule.Dispose();
                var joiningPlayersLeft = GameList.CheckJoinTimeOuts(JoinTimeOut);
                if (joiningPlayersLeft > 0)
                {
                    ExecutionFiber.Schedule(CheckJoinTimeOuts, (long)JoinTimeOut.TotalMilliseconds / 2);
                }
                else
                {
                    _checkJoinTimeoutSchedule = null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleRemoveGameServer(IncomingGameServerPeer gameServer)
        {
            try
            {
                GameList.RemoveGameServer(gameServer);
                SchedulePublishGameChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleRemoveGameState(string gameId)
        {
            try
            {
                GameState gameState;
                if (GameList.TryGetGame(gameId, out gameState) == false)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("HandleRemoveGameState: Game not found - gameId={0}", gameId);
                    }

                    return;
                }

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("HandleRemoveGameState: gameId={0}, joiningPlayers={1}", gameId,
                        gameState.JoiningPlayerCount);
                }

                GameList.RemoveGameState(gameId);
                SchedulePublishGameChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleRemovePeer(MasterClientPeer peer)
        {
            try
            {
                peer.GameChannelSubscription = null;
                peers.Remove(peer);
                OnRemovePeer(peer);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleUpdateGameState(UpdateGameEvent operation, IncomingGameServerPeer incomingGameServerPeer)
        {
            try
            {
                GameState gameState;

                if (GameList.UpdateGameState(operation, incomingGameServerPeer, out gameState) == false)
                {
                    return;
                }

                SchedulePublishGameChanges();

                OnGameStateChanged(gameState);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void SchedulePublishGameChanges()
        {
            if (_schedule == null)
            {
                _schedule = ExecutionFiber.Schedule(PublishGameChanges, 1000);
            }
        }

        private void PublishGameChanges()
        {
            try
            {
                _schedule = null;
                GameList.PublishGameChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private bool TryCreateGame(Operation operation, string gameId, bool createIfNotExists, Hashtable properties,
            out bool gameCreated, out GameState gameState, out OperationResponse errorResponse)
        {
            gameState = null;
            gameCreated = false;

            // try to get a game server instance from the load balancer            
            IncomingGameServerPeer gameServer;
            if (!Application.LoadBalancer.TryGetServer(out gameServer))
            {
                errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                {
                    ReturnCode = (int)ErrorCode.ServerFull,
                    DebugMessage = "Failed to get server instance."
                };

                return false;
            }

            // try to create or get game state
            if (createIfNotExists)
            {
                gameCreated = Application.GetOrCreateGame(gameId, this, GameList, (byte)MaxPlayersDefault, gameServer,
                    out gameState);
            }
            else
            {
                if (!Application.TryCreateGame(gameId, this, GameList, (byte)MaxPlayersDefault, gameServer,
                        out gameState))
                {
                    errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                    {
                        ReturnCode = (int)ErrorCode.GameIdAlreadyExists,
                        DebugMessage = "A game with the specified id already exist."
                    };

                    return false;
                }

                gameCreated = true;
            }


            if (properties != null)
            {
                bool changed;
                string debugMessage;

                if (!gameState.TrySetProperties(properties, out changed, out debugMessage))
                {
                    errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                    {
                        ReturnCode = (int)ErrorCode.OperationInvalid,
                        DebugMessage = debugMessage
                    };
                    return false;
                }
            }

            GameList.AddGameState(gameState);
            SchedulePublishGameChanges();

            errorResponse = null;
            return true;
        }

        #endregion
    }
}