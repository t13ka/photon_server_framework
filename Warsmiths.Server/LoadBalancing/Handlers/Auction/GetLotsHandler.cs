using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.ListContainer;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auction;
using Warsmiths.Server.Services.Auction;

namespace Warsmiths.Server.Handlers.Auction
{
    public class GetLotsHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.GetLots;

        /// <summary>
        /// </summary>
        private readonly AuctionRuntimeService _auction =
            ServiceManager.Get<AuctionRuntimeService>();

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var request = new GetLotsRequest(peerBase.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }
            var peer = (MasterClientPeer) peerBase;

            if (_auction.Count() > 0)
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.Ok,
                    DebugMessage = "auction data now in particular event",
                };

                //var lotsExceptPlayer = _auction.GetAll().Where(t => t.OwnerId != peer.CurrentPlayer._id).ToList();

                peer.SendUpdateAuctionEvent(new LotsListContainer {Lots = _auction.GetAll().ToList() });
            }
            else
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = "no lots"
                };
            }

            return response;
        }
    }
}