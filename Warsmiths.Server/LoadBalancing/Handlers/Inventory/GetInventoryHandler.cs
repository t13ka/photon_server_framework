using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Inventory;

namespace Warsmiths.Server.Handlers.Inventory
{
    public class GetInventoryHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.GetInventory;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;

            var peer = (MasterClientPeer)peerBase;

            var request = new GetInventoryRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            response =
                new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.Ok,
                        DebugMessage = "take inventory data in event"
                    };

            peer.SendUpdatePlayerInventoryEvent();

            return response;
        }
    }
}