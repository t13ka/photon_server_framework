using ExitGames.Logging;

using Photon.SocketServer;

using YourGame.Server.MasterServer;
using YourGame.Server.Operations.Request.Auth;
using YourGame.Server.Operations.Response;

namespace YourGame.Server.Handlers.Auth
{
    using YourGame.Common;
    using YourGame.Server.Framework.Handlers;

    public class AuthenticateHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.Authenticate;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer)peerBase;
            OperationResponse response;

            var request = new AuthenticateRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            // UserId = request.UserId;

            // publish operation response
            var responseObject = new AuthenticateResponse { QueuePosition = 0 };
            return new OperationResponse(operationRequest.OperationCode, responseObject);
        }
    }
}