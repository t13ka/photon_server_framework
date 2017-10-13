using System.Linq;

using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Common.Utils;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Economics;
using Warsmiths.Server.Operations.Response.Inventory;
using Warsmiths.Server.Services.Economic;

namespace Warsmiths.Server.Handlers.Economic
{
    public class OrderElementHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.OrderElement;

        private readonly EconomicRuntimeService _economics = ServiceManager.Get<EconomicRuntimeService>();

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new OrderElementRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var element =
                MasterApplication.DomainConfiguration.Elements.FirstOrDefault(t => t._id == request.ElementId);
            if (element == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                           {
                               ReturnCode =
                                   (short)ErrorCode.OperationFailed,
                               DebugMessage =
                                   $"Element with id {request.ElementId} not exists"
                           };
            }

            if (request.Quantity <= 0)
            {
                return new OperationResponse(operationRequest.OperationCode)
                           {
                               ReturnCode =
                                   (short)ErrorCode.OperationFailed,
                               DebugMessage =
                                   "Quantity for order should be more than zero"
                           };
            }

            var orderStatement = new OrderElementStatement(peer.CurrentPlayerId, request.ElementId, request.Quantity);

            var result = _economics.TransactionProcessing(orderStatement);

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