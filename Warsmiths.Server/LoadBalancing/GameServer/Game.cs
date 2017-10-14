using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Photon.SocketServer.Rpc.Protocols;

using Warsmiths.Server.Common;
using Warsmiths.Server.MasterServer.Lobby;
using Warsmiths.Server.Operations;
using Warsmiths.Server.Operations.Request;
using Warsmiths.Server.Operations.Response;
using Warsmiths.Server.ServerToServer.Events;

namespace Warsmiths.Server.GameServer
{
    using Warsmiths.Server.Operations.Request.GameManagement;

    using YourGame.Common;
    using YourGame.Server.Framework;
    using YourGame.Server.Framework.Diagnostics.OperationLogging;
    using YourGame.Server.Framework.Messages;
    using YourGame.Server.Framework.Operations;
    using YourGame.Server.GameServer;

    using OperationCode = YourGame.Server.Framework.Operations.OperationCode;

    public class Game : RoomGame
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private bool _isOpen = true;
        private bool _isVisible = true;

        /// <summary>
        ///     Contains the keys of the game properties hashtable which should be
        ///     listet in the lobby.
        /// </summary>
        private HashSet<object> _lobbyProperties;

        private AppLobbyType _lobbyType;
        private byte _maxPlayers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Game" /> class.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        /// <param name="roomCache">The room cache the game blongs to.</param>
        /// <param name="emptyRoomLiveTime">
        ///     A value indicating how long the room instance will be keeped alive
        ///     in the room cache after all peers have left the room.
        /// </param>
        public Game(string gameId, GameCache roomCache = null, int emptyRoomLiveTime = 0)
            : base(gameId, roomCache, emptyRoomLiveTime)
        {
            if (GameApplication.Instance.AppStatsPublisher != null)
            {
                GameApplication.Instance.AppStatsPublisher.IncrementGameCount();
            }
        }

        public string LobbyId { get; private set; }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                RemoveGameStateFromMaster();

                if (GameApplication.Instance.AppStatsPublisher != null)
                {
                    GameApplication.Instance.AppStatsPublisher.DecrementGameCount();
                }
            }
        }

        protected virtual Actor HandleJoinGameOperation(PlayerPeer peer, JoinRequest joinRequest,
            SendParameters sendParameters)
        {
            if (!ValidateGame(peer, joinRequest.OperationRequest, sendParameters))
            {
                return null;
            }

            // special treatment for game and actor properties sent by AS3/Flash
            var protocol = peer.Protocol.ProtocolType;
            if (protocol == ProtocolType.Amf3V152 || protocol == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(joinRequest.GameProperties, joinRequest.ActorProperties);
            }

            var gamePeer = (GameClientPeer) peer;
            var actor = HandleJoinOperation(peer, joinRequest, sendParameters);

            if (actor == null)
            {
                return null;
            }

            // update game state at master server            
            var peerId = gamePeer.PeerId ?? string.Empty;
            UpdateGameStateOnMaster(null, null, null, null, joinRequest.GameProperties, peerId);

            return actor;
        }

        protected virtual Actor HandleCreateGameOperation(PlayerPeer peer, JoinGameRequest createRequest,
            SendParameters sendParameters)
        {
            if (!ValidateGame(peer, createRequest.OperationRequest, sendParameters))
            {
                return null;
            }

            HandleCreateGameOperationBody(peer, createRequest, sendParameters, false);
            return null;
        }

        protected virtual void HandleCreateGameOperationBody(PlayerPeer peer, JoinGameRequest
            createRequest, SendParameters sendParameters, bool restored)
        {
            var gamePeer = (GameClientPeer) peer;

            byte? newMaxPlayer;
            bool? newIsOpen;
            bool? newIsVisible;
            object[] newLobbyProperties;
            var properties = createRequest.GameProperties;
            if (restored)
            {
                properties = Properties.GetProperties();

                /// we set it to null in order to prevent property update in HandleJoinOperation
                createRequest.GameProperties = null;
            }

            SetupGameProperties(peer, createRequest, properties,
                ref sendParameters, out newMaxPlayer, out newIsOpen, out newIsVisible, out newLobbyProperties);

            var actor = HandleJoinOperation(peer, createRequest, sendParameters);
            if (actor == null)
            {
                return;
            }

            LobbyId = createRequest.LobbyName;
            _lobbyType = (AppLobbyType) createRequest.LobbyType;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("CreateGame: name={0}, lobyName={1}, lobbyType={2}", Name, LobbyId, _lobbyType);
            }

            // set default properties
            if (newMaxPlayer.HasValue && newMaxPlayer.Value != _maxPlayers)
            {
                _maxPlayers = newMaxPlayer.Value;
            }

            if (newIsOpen.HasValue && newIsOpen.Value != _isOpen)
            {
                _isOpen = newIsOpen.Value;
            }

            if (newIsVisible.HasValue && newIsVisible.Value != _isVisible)
            {
                _isVisible = newIsVisible.Value;
            }

            if (newLobbyProperties != null)
            {
                _lobbyProperties = new HashSet<object>(newLobbyProperties);
            }

            // update game state at master server            
            var peerId = gamePeer.PeerId ?? string.Empty;
            var gameProperties = GetLobbyGameProperties(properties);
            UpdateGameStateOnMaster(newMaxPlayer, newIsOpen, newIsVisible, newLobbyProperties, gameProperties, peerId);
        }

        private void SetupGameProperties(PlayerPeer peer, JoinGameRequest createRequest,
            Hashtable gameProperties, ref SendParameters sendParameters, out byte? newMaxPlayer, out bool? newIsOpen,
            out bool? newIsVisible, out object[] newLobbyProperties)
        {
            newMaxPlayer = null;
            newIsOpen = null;
            newIsVisible = null;
            newLobbyProperties = null;

            // special treatment for game and actor properties sent by AS3/Flash or JSON clients
            var protocol = peer.Protocol.ProtocolType;
            if (protocol == ProtocolType.Amf3V152 || protocol == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(createRequest.GameProperties, createRequest.ActorProperties);
            }

            // try to parse build in properties for the first actor (creator of the game)
            if (Actors.Count == 0)
            {
                if (gameProperties != null && gameProperties.Count > 0)
                {
                    if (!TryParseDefaultProperties(peer, createRequest, gameProperties,
                        sendParameters, out newMaxPlayer, out newIsOpen, out newIsVisible, out newLobbyProperties))
                    {
                    }
                }
            }
        }

        protected override int RemovePeerFromGame(PlayerPeer peer, LeaveRequest leaveRequest)
        {
            var result = base.RemovePeerFromGame(peer, leaveRequest);

            if (IsDisposed)
            {
                return result;
            }

            // If there are still peers left an UpdateGameStateOperation with the new 
            // actor count will be send to the master server.
            // If there are no actors left the RoomCache will dispose this instance and a 
            // RemoveGameStateOperation will be sent to the master.
            if (Actors.Count > 0)
            {
                var gamePeer = (GameClientPeer) peer;
                var peerId = gamePeer.PeerId ?? string.Empty;
                UpdateGameStateOnMaster(null, null, null, null, null, null, peerId);
            }

            return result;
        }

        protected override void HandleGetPropertiesOperation(PlayerPeer peer, GetPropertiesRequest getPropertiesRequest,
            SendParameters sendParameters)
        {
            // special handling for game properties send by AS3/Flash (Amf3 protocol) clients or JSON clients
            if (peer.Protocol.ProtocolType == ProtocolType.Amf3V152 || peer.Protocol.ProtocolType == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(getPropertiesRequest.GamePropertyKeys,
                    getPropertiesRequest.ActorPropertyKeys);
            }

            base.HandleGetPropertiesOperation(peer, getPropertiesRequest, sendParameters);
        }

        protected override void HandleSetPropertiesOperation(PlayerPeer peer, SetPropertiesRequest request,
            SendParameters sendParameters)
        {
            byte? newMaxPlayer = null;
            bool? newIsOpen = null;
            bool? newIsVisible = null;
            object[] newLobbyProperties = null;

            // try to parse build in propeties if game properties should be set (ActorNumber == 0)
            var updateGameProperties = request.ActorNumber == 0 && request.Properties != null &&
                                       request.Properties.Count > 0;

            // special handling for game and actor properties send by AS3/Flash (Amf3 protocol) or JSON clients
            if (peer.Protocol.ProtocolType == ProtocolType.Amf3V152 || peer.Protocol.ProtocolType == ProtocolType.Json)
            {
                if (updateGameProperties)
                {
                    Utilities.ConvertAs3WellKnownPropertyKeys(request.Properties, null);
                }
                else
                {
                    Utilities.ConvertAs3WellKnownPropertyKeys(null, request.Properties);
                }
            }

            if (updateGameProperties)
            {
                if (
                    !TryParseDefaultProperties(peer, request, request.Properties, sendParameters, out newMaxPlayer,
                        out newIsOpen, out newIsVisible, out newLobbyProperties))
                {
                    return;
                }
            }

            base.HandleSetPropertiesOperation(peer, request, sendParameters);

            // report to master only if game properties are updated
            if (!updateGameProperties)
            {
                return;
            }

            // set default properties
            if (newMaxPlayer.HasValue && newMaxPlayer.Value != _maxPlayers)
            {
                _maxPlayers = newMaxPlayer.Value;
            }

            if (newIsOpen.HasValue && newIsOpen.Value != _isOpen)
            {
                _isOpen = newIsOpen.Value;
            }

            if (newIsVisible.HasValue && newIsVisible.Value != _isVisible)
            {
                _isVisible = newIsVisible.Value;
            }

            if (newLobbyProperties != null)
            {
                _lobbyProperties = new HashSet<object>(newLobbyProperties);
            }

            Hashtable gameProperties;
            if (newLobbyProperties != null)
            {
                // if the property filter for the app lobby properties has been changed
                // all game properties are resend to the master server because the application 
                // lobby might not contain all properties specified.
                gameProperties = GetLobbyGameProperties(Properties.GetProperties());
            }
            else
            {
                // property filter hasn't chjanged; only the changed properties will
                // be updatet in the application lobby
                gameProperties = GetLobbyGameProperties(request.Properties);
            }

            UpdateGameStateOnMaster(newMaxPlayer, newIsOpen, newIsVisible, newLobbyProperties, gameProperties);
        }

        protected virtual void HandleDebugGameOperation(PlayerPeer peer, DebugGameRequest request,
            SendParameters sendParameters)
        {
            // Room: Properties; # of cached events
            // Actors:  Count, Last Activity, Actor #, Peer State, Connection ID
            // Room Reference

            // get info from request (was gathered in Peer class before operation was forwarded to the game): 
            var peerInfo = request.Info;
            var debugInfo = peerInfo + this;

            if (Log.IsInfoEnabled)
            {
                Log.Info("DebugGame: " + debugInfo);
            }

            LogQueue.WriteLog();

            var debugGameResponse = new DebugGameResponse {Info = debugInfo};

            peer.SendOperationResponse(
                new OperationResponse(request.OperationRequest.OperationCode, debugGameResponse), sendParameters);
        }

        protected override void ExecuteOperation(PlayerPeer peer, OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Executing operation {0}", operationRequest.OperationCode);
                }

                switch (operationRequest.OperationCode)
                {
                    case (byte) OperationCode.CreateGame:
                        var createGameRequest = new JoinGameRequest(peer.Protocol, operationRequest);
                        if (peer.ValidateOperation(createGameRequest, sendParameters) == false)
                        {
                            return;
                        }

                        if (LogQueue.Log.IsDebugEnabled)
                        {
                            LogQueue.Add(
                                new LogEntry(
                                    "ExecuteOperation: " + (OperationCode) operationRequest.OperationCode,
                                    "Peer=" + peer.ConnectionId));
                        }
                        HandleCreateGameOperation(peer, createGameRequest, sendParameters);
                        break;

                    case (byte) OperationCode.JoinGame:
                        var joinGameRequest = new JoinRequest(peer.Protocol, operationRequest);
                        if (peer.ValidateOperation(joinGameRequest, sendParameters) == false)
                        {
                            return;
                        }

                        if (LogQueue.Log.IsDebugEnabled)
                        {
                            LogQueue.Add(
                                new LogEntry(
                                    "ExecuteOperation: " + (OperationCode) operationRequest.OperationCode,
                                    "Peer=" + peer.ConnectionId));
                        }
                        HandleJoinGameOperation(peer, joinGameRequest, sendParameters);
                        break;

                    // Lite operation code for join is not allowed in load balanced games.
                    case (byte) OperationCode.Join:
                        var response = new OperationResponse
                        {
                            OperationCode = operationRequest.OperationCode,
                            ReturnCode = (short) ErrorCode.OperationDenied,
                            DebugMessage = "Invalid operation code"
                        };
                        peer.SendOperationResponse(response, sendParameters);
                        break;

                    case (byte) OperationCode.DebugGame:
                        var debugGameRequest = new DebugGameRequest(peer.Protocol, operationRequest);
                        if (peer.ValidateOperation(debugGameRequest, sendParameters) == false)
                        {
                            return;
                        }

                        if (LogQueue.Log.IsDebugEnabled)
                        {
                            LogQueue.Add(
                                new LogEntry(
                                    "ExecuteOperation: " + (OperationCode) operationRequest.OperationCode,
                                    "Peer=" + peer.ConnectionId));
                        }

                        HandleDebugGameOperation(peer, debugGameRequest, sendParameters);
                        break;

                    // all other operation codes will be handled by the base game implementation
                    default:
                        base.ExecuteOperation(peer, operationRequest, sendParameters);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected override void ProcessMessage(IMessage message)
        {
            try
            {
                switch (message.Action)
                {
                    case (byte) GameMessageCodes.ReinitializeGameStateOnMaster:
                        if (Actors.Count == 0)
                        {
                            Log.WarnFormat("Reinitialize tried to update GameState with ActorCount = 0. " + this);
                        }
                        else
                        {
                            var gameProperties = GetLobbyGameProperties(Properties.GetProperties());
                            object[] lobbyPropertyFilter = null;
                            if (_lobbyProperties != null)
                            {
                                lobbyPropertyFilter = new object[_lobbyProperties.Count];
                                _lobbyProperties.CopyTo(lobbyPropertyFilter);
                            }

                            UpdateGameStateOnMaster(_maxPlayers, _isOpen, _isVisible, lobbyPropertyFilter,
                                gameProperties, null, null, true);
                        }

                        break;

                    case (byte) GameMessageCodes.CheckGame:
                        CheckGame();
                        break;

                    default:
                        base.ProcessMessage(message);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     <see cref="Uri.Check" /> routine to validate that the game is valid
        ///     (ie., it is removed from the game cache if it has no longer any
        ///     actors etc.). CheckGame() is called by the Application at regular
        ///     intervals.
        /// </summary>
        protected virtual void CheckGame()
        {
            if (Actors.Count == 0)
            {
                // double check if the game is still in cache: 
                Room room;
                if (GameCache.Instance.TryGetRoomWithoutReference(Name, out room))
                {
                    Log.WarnFormat("Game with 0 Actors is still in cache: {0}",
                        GameCache.Instance.GetDebugString(room.Name));
                }
            }
        }

        protected virtual void UpdateGameStateOnMaster(
            byte? newMaxPlayer = null,
            bool? newIsOpen = null,
            bool? newIsVisble = null,
            object[] lobbyPropertyFilter = null,
            Hashtable gameProperties = null,
            string newPeerId = null,
            string removedPeerId = null,
            bool reinitialize = false)
        {
            if (reinitialize && Actors.Count == 0)
            {
                Log.WarnFormat("Reinitialize tried to update GameState with ActorCount = 0. " + this);
                return;
            }

            var updateGameEvent = new UpdateGameEvent
            {
                GameId = Name,
                ActorCount = (byte) Actors.Count,
                Reinitialize = reinitialize,
                MaxPlayers = newMaxPlayer,
                IsOpen = newIsOpen,
                IsVisible = newIsVisble,
                PropertyFilter = lobbyPropertyFilter
            };

            if (reinitialize)
            {
                updateGameEvent.LobbyId = LobbyId;
                updateGameEvent.LobbyType = (byte) _lobbyType;
            }

            if (gameProperties != null && gameProperties.Count > 0)
            {
                updateGameEvent.GameProperties = gameProperties;
            }

            if (newPeerId != null)
            {
                updateGameEvent.NewUsers = new[] {newPeerId};
            }

            if (removedPeerId != null)
            {
                updateGameEvent.RemovedUsers = new[] {removedPeerId};
            }

            UpdateGameStateOnMaster(updateGameEvent);
        }

        protected virtual void UpdateGameStateOnMaster(UpdateGameEvent updateEvent)
        {
            var eventData = new EventData((byte) ServerEventCode.UpdateGameState, updateEvent);
            GameApplication.Instance.MasterPeer.SendEvent(eventData, new SendParameters());
        }

        protected virtual void RemoveGameStateFromMaster()
        {
            GameApplication.Instance.MasterPeer.RemoveGameState(Name);
        }

        private static bool TryParseDefaultProperties(
            PlayerPeer peer, Operation operation, Hashtable propertyTable, SendParameters sendParameters,
            out byte? maxPlayer, out bool? isOpen, out bool? isVisible, out object[] properties)
        {
            string debugMessage;

            if (
                !GameParameterReader.TryReadDefaultParameter(propertyTable, out maxPlayer, out isOpen, out isVisible,
                    out properties, out debugMessage))
            {
                var response = new OperationResponse
                {
                    OperationCode = operation.OperationRequest.OperationCode,
                    ReturnCode = (short)ErrorCode.OperationInvalid,
                    DebugMessage = debugMessage
                };
                peer.SendOperationResponse(response, sendParameters);
                return false;
            }

            return true;
        }

        protected bool ValidateGame(PlayerPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var gamePeer = (GameClientPeer) peer;

            // check if the game is open
            if (_isOpen == false)
            {
                var errorResponse = new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (int) ErrorCode.GameClosed,
                    DebugMessage = "Game closed"
                };
                peer.SendOperationResponse(errorResponse, sendParameters);
                gamePeer.OnJoinFailed(ErrorCode.GameClosed);
                return false;
            }

            // check if the maximum number of players has already been reached
            if (_maxPlayers > 0 && Actors.Count >= _maxPlayers)
            {
                var errorResponse = new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (int) ErrorCode.GameFull,
                    DebugMessage = "Game full"
                };
                peer.SendOperationResponse(errorResponse, sendParameters);
                gamePeer.OnJoinFailed(ErrorCode.GameFull);
                return false;
            }

            return true;
        }

        private Hashtable GetLobbyGameProperties(Hashtable source)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }

            Hashtable gameProperties;

            if (_lobbyProperties != null)
            {
                // filter for game properties is set, only properties in the specified list 
                // will be reported to the lobby 
                gameProperties = new Hashtable(_lobbyProperties.Count);

                foreach (var entry in _lobbyProperties)
                {
                    if (source.ContainsKey(entry))
                    {
                        gameProperties.Add(entry, source[entry]);
                    }
                }
            }
            else
            {
                // if no filter is set for properties which should be listet in the lobby
                // all properties are send
                gameProperties = source;
                gameProperties.Remove((byte) GameParameter.MaxPlayer);
                gameProperties.Remove((byte) GameParameter.IsOpen);
                gameProperties.Remove((byte) GameParameter.IsVisible);
                gameProperties.Remove((byte) GameParameter.Properties);
            }

            return gameProperties;
        }
    }
}