using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auction;
using Warsmiths.Server.Operations.Response.Auction;
using Warsmiths.Server.Services.Auction;

namespace Warsmiths.Server.Handlers.Auction
{
    public class PublishLotHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly AuctionRuntimeService _auction =
            ServiceManager.Get<AuctionRuntimeService>();

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.PublishLot;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;

            var request = new PublishLotRequest(peerBase.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            if (request.Money <= 0)
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = "Price should be more than zero!"
                };
            }

            var peer = (MasterClientPeer) peerBase;
            var currentPlayer = peer.GetCurrentPlayer();
            var checkLot = _auction.GetLotByEquipmentId(request.EquipmentId);
            if (checkLot != null && checkLot.OwnerId == currentPlayer._id)
            {
                return new OperationResponse(operationRequest.OperationCode,
                    new AuctionResponse { LotId = request.EquipmentId })
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = "the lot is already published!"
                };
            }

            IEntity item;

            if (currentPlayer.Inventory.TryGetValue(request.EquipmentId, out item) == false)
            {
                response = new OperationResponse(operationRequest.OperationCode,
                    new AuctionResponse {LotId = request.EquipmentId})
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = "Player does not have this equipment in inventory"
                };
            }
            else
            {
                var lot = new Lot();
                lot.Put(item, request.Money);
                _auction.Publish(lot);

                // remove from inventory
                currentPlayer.TryRemoveFromInventory(request.EquipmentId);

                _playerRepository.Update(currentPlayer);

                response = new OperationResponse(operationRequest.OperationCode,
                    new AuctionResponse {LotId = request.EquipmentId})
                {
                    ReturnCode = (short) ErrorCode.Ok,
                    DebugMessage = "the lot has been successfully published"
                };

                // send to all
                peer.SendUpdatePlayerInventoryEvent();
                _auction.SendUpdateAuctionDataToSubscribers();
            }

            return response;
        }
    }
}