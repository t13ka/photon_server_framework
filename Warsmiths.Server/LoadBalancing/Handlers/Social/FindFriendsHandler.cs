using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Social;

namespace Warsmiths.Server.Handlers.Social
{
    public class FindFriendsHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.FiendFriends;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer)peerBase;

            // validate the operation request
            OperationResponse response;
            var operation = new FindFriendsRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, _log, out response) == false)
            {
                return response;
            }

            // check if player online cache is available for the application
            var playerCache = peer.Application.PlayerOnlineCache;
            if (playerCache == null)
            {
                return new OperationResponse((byte)OperationCode.FiendFriends)
                           {
                               ReturnCode =
                                   (short)ErrorCode.OperationDenied,
                               DebugMessage =
                                   "Friend list not available"
                           };
            }

            playerCache.FiendFriends(peer, operation, sendParameters);
            return null;
        }
    }
}