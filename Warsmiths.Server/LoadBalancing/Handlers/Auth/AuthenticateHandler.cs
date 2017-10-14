using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auth;
using Warsmiths.Server.Operations.Response;

namespace Warsmiths.Server.Handlers.Auth
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