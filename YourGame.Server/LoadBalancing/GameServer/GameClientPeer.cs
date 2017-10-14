using System;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

using YourGame.Server.Operations.Request.Auth;

namespace YourGame.Server.GameServer
{
    using YourGame.Server.Operations.Request.GameManagement;

    using YourGame.Common;
    using YourGame.Server.Framework;
    using YourGame.Server.Framework.Caching;
    using YourGame.Server.Framework.Messages;
    using YourGame.Server.Framework.Operations;
    using YourGame.Server.GameServer;

    using OperationCode = YourGame.Server.Framework.Operations.OperationCode;
    using ParameterCode = YourGame.Server.Operations.ParameterCode;

    public class GameClientPeer : PlayerPeer
    {
        #region Constructors and Destructors

        public GameClientPeer(InitRequest initRequest, GameApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            _application = application;
            PeerId = string.Empty;

            if (GameApplication.Instance.AppStatsPublisher != null)
            {
                GameApplication.Instance.AppStatsPublisher.IncrementPeerCount();
            }
        }

        #endregion

        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly GameApplication _application;

        #endregion

        #region Properties

        public string PeerId { get; protected set; }

        public DateTime LastActivity { get; protected set; }

        public byte LastOperation { get; protected set; }

        #endregion

        #region Public Methods

        public void OnJoinFailed(ErrorCode result)
        {
            RequestFiber.Enqueue(() => OnJoinFailedInternal(result));
        }

        public override string ToString()
        {
            return string.Format(
                "{0}: {1}",
                GetType().Name,
                string.Format(
                    "PID {0}, IsConnected: {1}, IsDisposed: {2}, Last Activity: Operation {3} at UTC {4} in Room {7}, IP {5}:{6}, ",
                    ConnectionId,
                    Connected,
                    Disposed,
                    LastOperation,
                    LastActivity,
                    RemoteIP,
                    RemotePort,
                    RoomReference == null ? string.Empty : RoomReference.Room.Name));
        }

        #endregion

        #region Methods

        protected override RoomReference GetRoomReference(JoinRequest joinRequest)
        {
            throw new NotSupportedException("Use TryGetRoomReference or TryCreateRoomReference instead.");
        }

        protected virtual void HandleCreateGameOperation(OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            // The JoinRequest from the Lite application is also used for create game operations to support all feaures 
            // provided by Lite games. 
            // The only difference is the operation code to prevent games created by a join operation. 
            // On "LoadBalancing" game servers games must by created first by the game creator to ensure that no other joining peer 
            // reaches the game server before the game is created.
            var createRequest = new JoinGameRequest(Protocol, operationRequest);
            if (ValidateOperation(createRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            RemovePeerFromCurrentRoom();

            // try to create the game
            RoomReference gameReference;
            if (TryCreateRoom(createRequest.GameId, out gameReference) == false)
            {
                var response = new OperationResponse
                {
                    OperationCode = (byte) OperationCode.CreateGame,
                    ReturnCode = (short) ErrorCode.GameIdAlreadyExists,
                    DebugMessage = "Game already exists"
                };

                SendOperationResponse(response, sendParameters);
                return;
            }

            // save the game reference in the peers state                    
            RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        /// <summary>
        ///     Handles the <see cref="JoinRequest" /> to enter a <see cref="Game" />.
        ///     This method removes the peer from any previously joined room, finds the room intended for join
        ///     and enqueues the operation for it to handle.
        /// </summary>
        /// <param name="operationRequest">
        ///     The operation request to handle.
        /// </param>
        /// <param name="sendParameters">
        ///     The send Parameters.
        /// </param>
        protected virtual void HandleJoinGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // create join operation
            var joinRequest = new JoinGameRequest(Protocol, operationRequest);
            if (ValidateOperation(joinRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            RemovePeerFromCurrentRoom();

            // try to get the game reference from the game cache 
            RoomReference gameReference;
            if (joinRequest.CreateIfNotExists)
            {
                gameReference = GetOrCreateRoom(joinRequest.GameId);
            }
            else
            {
                if (TryGetRoomReference(joinRequest.GameId, out gameReference) == false)
                {
                    OnRoomNotFound(joinRequest.GameId);

                    var response = new OperationResponse
                    {
                        OperationCode = (byte) OperationCode.JoinGame,
                        ReturnCode = (short) ErrorCode.GameIdNotExists,
                        DebugMessage = "Game does not exists"
                    };

                    SendOperationResponse(response, sendParameters);
                    return;
                }
            }

            // save the game reference in the peers state                    
            RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        protected virtual void HandleDebugGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var debugRequest = new DebugGameRequest(Protocol, operationRequest);
            if (ValidateOperation(debugRequest, sendParameters) == false)
            {
                return;
            }

            var debug = string.Format("DebugGame called from PID {0}. {1}", ConnectionId,
                GetRoomCacheDebugString(debugRequest.GameId));
            operationRequest.Parameters.Add((byte) ParameterCode.Info, debug);


            if (RoomReference == null)
            {
                Room room;
                // get a room without obtaining a reference:
                if (!TryGetRoomWithoutReference(debugRequest.GameId, out room))
                {
                    var response = new OperationResponse
                    {
                        OperationCode = (byte) OperationCode.DebugGame,
                        ReturnCode = (short) ErrorCode.GameIdNotExists,
                        DebugMessage = "Game does not exist in RoomCache"
                    };


                    SendOperationResponse(response, sendParameters);
                    return;
                }

                room.EnqueueOperation(this, operationRequest, sendParameters);
            }
            else
            {
                RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
            }
        }

        protected virtual void OnRoomNotFound(string gameId)
        {
            _application.MasterPeer.RemoveGameState(gameId);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnDisconnect: conId={0}, reason={1}, detail={2}", ConnectionId, reasonCode,
                    reasonDetail);
            }

            if (GameApplication.Instance.AppStatsPublisher != null)
            {
                GameApplication.Instance.AppStatsPublisher.DecrementPeerCount();
            }

            if (RoomReference == null)
            {
                return;
            }

            var message = new RoomMessage((byte) GameMessageCodes.RemovePeerFromGame, this);
            RoomReference.Room.EnqueueMessage(message);
            RoomReference.Dispose();
            RoomReference = null;
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                if (request.OperationCode != (byte) OperationCode.RaiseEvent)
                {
                    log.DebugFormat("OnOperationRequest: conId={0}, opCode={1}", ConnectionId, request.OperationCode);
                }
            }

            LastActivity = DateTime.UtcNow;
            LastOperation = request.OperationCode;


            switch (request.OperationCode)
            {
                case (byte) OperationCode.Authenticate:
                    HandleAuthenticateOperation(request, sendParameters);
                    return;

                case (byte) OperationCode.CreateGame:
                    HandleCreateGameOperation(request, sendParameters);
                    return;

                case (byte) OperationCode.JoinGame:
                    HandleJoinGameOperation(request, sendParameters);
                    return;

                case (byte) OperationCode.Leave:
                    HandleLeaveOperation(request, sendParameters);
                    return;

                case (byte) OperationCode.Ping:
                    HandlePingOperation(request, sendParameters);
                    return;

                case (byte) OperationCode.DebugGame:
                    HandleDebugGameOperation(request, sendParameters);
                    return;

                case (byte) OperationCode.RaiseEvent:
                case (byte) OperationCode.GetProperties:
                case (byte) OperationCode.SetProperties:
                case (byte) OperationCode.ChangeGroups:
                    HandleGameOperation(request, sendParameters);
                    return;
            }

            var message = $"Unknown operation code {request.OperationCode}";
            var response = new OperationResponse
            {
                ReturnCode = (short) ErrorCode.OperationDenied,
                DebugMessage = message,
                OperationCode = request.OperationCode
            };
            SendOperationResponse(response, sendParameters);
        }

        protected virtual RoomReference GetOrCreateRoom(string gameId)
        {
            return GameCache.Instance.GetRoomReference(gameId, this);
        }

        protected virtual bool TryCreateRoom(string gameId, out RoomReference roomReference)
        {
            return GameCache.Instance.TryCreateRoom(gameId, this, out roomReference);
        }

        protected virtual bool TryGetRoomReference(string gameId, out RoomReference roomReference)
        {
            return GameCache.Instance.TryGetRoomReference(gameId, this, out roomReference);
        }

        protected virtual bool TryGetRoomWithoutReference(string gameId, out Room room)
        {
            return GameCache.Instance.TryGetRoomWithoutReference(gameId, out room);
        }

        public virtual string GetRoomCacheDebugString(string gameId)
        {
            return GameCache.Instance.GetDebugString(gameId);
        }

        protected virtual void HandleAuthenticateOperation(OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            var request = new AuthenticateRequest(Protocol, operationRequest);
            if (ValidateOperation(request, sendParameters) == false)
            {
                return;
            }

            if (request.UserId != null)
            {
                PeerId = request.UserId;
            }

            var response = new OperationResponse {OperationCode = operationRequest.OperationCode};
            SendOperationResponse(response, sendParameters);
        }

        private void OnJoinFailedInternal(ErrorCode result)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnJoinFailed: {0}", result);
            }

            // if join operation failed -> release the reference to the room
            if (result != ErrorCode.Ok && RoomReference != null)
            {
                RoomReference.Dispose();
                RoomReference = null;
            }
        }

        #endregion
    }
}