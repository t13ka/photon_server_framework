using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Handlers.GameManagement
{
    public class LeaveLobbyHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.LeaveLobby;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer)peerBase;
            peer.GameChannelSubscription = null;

            if (peer.AppLobby == null)
            {
                return new OperationResponse
                           {
                               OperationCode = operationRequest.OperationCode,
                               ReturnCode = 0,
                               DebugMessage = "lobby not joined"
                           };
            }

            peer.AppLobby.RemovePeer(peer);
            peer.AppLobby = null;

            return new OperationResponse(operationRequest.OperationCode);
        }
    }
}