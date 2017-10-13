using System;
using System.Linq;

using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Common.Domain.Elements;
using Warsmiths.Common.ListContainer;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Economics;
using Warsmiths.Server.Services.Economic;

namespace Warsmiths.Server.Handlers.Economic
{
    public class GetElementPricesHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.GetElementPrices;

        private readonly EconomicRuntimeService _economics = ServiceManager.Get<EconomicRuntimeService>();

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new GeElementPricesRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var container = new ElementsPricesContainer
                                {
                                    ElementPriceItemResults = _economics.GlobalElementStatements
                                        .Select(
                                            t => new ElementPriceItemResult
                                                     {
                                                         BuyPrice =
                                                             t.BuyPrice,
                                                         SellPrice =
                                                             t
                                                                 .OrderPrice,
                                                         ElementId =
                                                             t.Element
                                                                 ._id
                                                     })
                                        .ToList()
                                };

            peer.SendElementPricesEvent(container);

            return new OperationResponse(operationRequest.OperationCode)
                       {
                           ReturnCode = (short)ErrorCode.Ok,
                           DebugMessage = "element prices were sent"
                       };
        }
    }
}