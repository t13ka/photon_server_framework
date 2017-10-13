using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Economics;
using Warsmiths.Server.Operations.Response.Inventory;
using Warsmiths.Server.Services.Economic;

namespace Warsmiths.Server.Handlers.Economic
{
    public class SellElementHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.SellElement;

        private readonly EconomicRuntimeService _economics = ServiceManager.Get<EconomicRuntimeService>();

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new SellElementRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var sellStatement = new SaleElementStatement(peer.CurrentPlayerId, request.ElementId, request.Quantity);

            if (request.Quantity <= 0)
            {
                return new OperationResponse(operationRequest.OperationCode)
                           {
                               ReturnCode =
                                   (short)ErrorCode.OperationFailed,
                               DebugMessage =
                                   "what fucking you doing?"
                           };
            }

            var result = _economics.TransactionProcessing(sellStatement);

            if (result.Success)
            {
                response = new OperationResponse(
                               operationRequest.OperationCode,
                               new InventoryResponse { EntityId = request.ElementId })
                               {
                                   ReturnCode =
                                       (short)ErrorCode.Ok,
                                   DebugMessage = result
                                       .DebugMessage
                               };

                peer.SendUpdatePlayerProfileEvent();
                peer.SendUpdateElementsOrderEvent();
            }
            else
            {
                response =
                    new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode =
                                (short)ErrorCode.OperationFailed,
                            DebugMessage = result.DebugMessage
                        };
            }

            return response;
        }
    }
}