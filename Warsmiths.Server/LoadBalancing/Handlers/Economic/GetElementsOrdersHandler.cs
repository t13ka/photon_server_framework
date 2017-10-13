using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Handlers.Economic
{
    public class GetElementsOrdersHandler : BaseHandler
    {
        public override OperationCode ControlCode => OperationCode.GetElementsOrders;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer) peerBase;

            var response = new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            peer.SendUpdateElementsOrderEvent();
            
            return response;
        }
    }
}