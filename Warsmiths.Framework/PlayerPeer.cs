using ExitGames.Logging;

using log4net.Core;

using Photon.SocketServer;
using Photon.SocketServer.Rpc;

using PhotonHostRuntimeInterfaces;

using Warsmiths.Server.Framework.Caching;
using Warsmiths.Server.Framework.Messages;
using Warsmiths.Server.Framework.Operations;

using ILogger = ExitGames.Logging.ILogger;

namespace Warsmiths.Server.Framework
{
    public class PlayerPeer : PeerBase
    {
        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        public PlayerPeer(IRpcProtocol rpcProtocol, IPhotonPeer nativePeer)
            : base(rpcProtocol, nativePeer)
        {
        }

        #endregion

        #region Properties

        public RoomReference RoomReference;

        #endregion

        #region Public Methods

        public bool ValidateOperation(Operation operation, SendParameters sendParameters)
        {
            if (operation.IsValid)
            {
                return true;
            }

            var errorMessage = operation.GetErrorMessage();
            SendOperationResponse(
                new OperationResponse
                    {
                        OperationCode = operation.OperationRequest.OperationCode,
                        ReturnCode = -1,
                        DebugMessage = errorMessage
                    },
                sendParameters);
            return false;
        }

        #endregion

        #region Methods

        protected virtual RoomReference GetRoomReference(JoinRequest joinRequest)
        {
            return RoomGameCache.Instance.GetRoomReference(joinRequest.GameId, this);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(
                    "OnDisconnect: conId={0}, reason={1}, reasonDetail={2}",
                    ConnectionId,
                    reasonCode,
                    reasonDetail);
            }

            if (RoomReference == null)
            {
                return;
            }

            var message = new RoomMessage((byte)GameMessageCodes.RemovePeerFromGame, this);
            RoomReference.Room.EnqueueMessage(message);
            RoomReference.Dispose();
            RoomReference = null;
        }

        protected override void OnOperationRequest(
            OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("OnOperationRequest. Code={0}", operationRequest.OperationCode);
            }

            switch ((OperationCode)operationRequest.OperationCode)
            {
                case OperationCode.Ping:
                    HandlePingOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.Join:
                    HandleJoinOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.Leave:
                    HandleLeaveOperation(operationRequest, sendParameters);
                    return;

                case OperationCode.RaiseEvent:
                case OperationCode.GetProperties:
                case OperationCode.SetProperties:
                case OperationCode.ChangeGroups:
                    HandleGameOperation(operationRequest, sendParameters);
                    return;
            }

            var message = $"Unknown operation code {operationRequest.OperationCode}";
            SendOperationResponse(
                new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode,
                        ReturnCode = -1,
                        DebugMessage = message
                    },
                sendParameters);
        }

        protected virtual void RemovePeerFromCurrentRoom()
        {
            // check if the peer already joined another game
            if (RoomReference != null)
            {
                // remove peer from his current game.
                var message = new RoomMessage((byte)GameMessageCodes.RemovePeerFromGame, this);
                RoomReference.Room.EnqueueMessage(message);

                // release room reference
                RoomReference.Dispose();
                RoomReference = null;
            }
        }

        #endregion

        #region Handlers

        protected virtual void HandleGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // enqueue operation into game queue. 
            // the operation request will be processed in the games ExecuteOperation method.
            if (RoomReference != null)
            {
                RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
                return;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Received game operation on peer without a game: peerId={0}", ConnectionId);
            }
        }

        protected virtual void HandleJoinOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // create join operation
            var joinRequest = new JoinRequest(Protocol, operationRequest);
            if (ValidateOperation(joinRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            RemovePeerFromCurrentRoom();

            // get a game reference from the game cache 
            // the game will be created by the cache if it does not exists already
            var gameReference = GetRoomReference(joinRequest);

            // save the game reference in the peers state                    
            RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        protected virtual void HandleLeaveOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // check if the peer have a reference to game 
            if (RoomReference == null)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Received leave operation on peer without a game: peerId={0}", ConnectionId);
                }

                return;
            }

            // enqueue the leave operation into game queue. 
            RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);

            // release the reference to the game
            // the game cache will recycle the game instance if no 
            // more refrences to the game are left.
            RoomReference.Dispose();

            // finally the peers state is set to null to indicate
            // that the peer is not attached to a room anymore.
            RoomReference = null;
        }

        protected virtual void HandlePingOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            SendOperationResponse(
                new OperationResponse { OperationCode = operationRequest.OperationCode },
                sendParameters);
        }

        #endregion
    }
}