using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auth;

namespace Warsmiths.Server.Handlers.Player
{
    public class GetProfileHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.GetProfile;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new GetProfileRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            response =
                new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.Ok,
                        DebugMessage =
                            "profile successfully extracted and sent"
                    };

            peer.SendUpdatePlayerProfileEvent();

            return response;
        }
    }
}