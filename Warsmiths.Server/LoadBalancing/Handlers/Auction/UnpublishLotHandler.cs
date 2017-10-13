using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auction;
using Warsmiths.Server.Services.Auction;

namespace Warsmiths.Server.Handlers.Auction
{
    public class UnpublishLotHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly AuctionRuntimeService _auction = ServiceManager.Get<AuctionRuntimeService>();

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.UnpublishLot;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;

            var request = new UnpublishLotRequest(peerBase.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var peer = (MasterClientPeer)peerBase;

            var lot = _auction.GetLotByEquipmentId(request.LotId);
            if (lot == null)
            {
                response =
                    new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode =
                                (short)ErrorCode.OperationFailed,
                            DebugMessage = "the lot not found"
                        };
            }
            else
            {
                var currentPlayer = peer.GetCurrentPlayer();
                if (lot.OwnerId != currentPlayer._id)
                {
                    response =
                        new OperationResponse(operationRequest.OperationCode)
                            {
                                ReturnCode =
                                    (short)ErrorCode.OperationFailed,
                                DebugMessage =
                                    "current player are not owner for this lot"
                            };
                }
                else
                {
                    _auction.Unpublish(lot);

                    currentPlayer.TryAddToInventory(lot.Entity);

                    _playerRepository.Update(currentPlayer);

                    response =
                        new OperationResponse(operationRequest.OperationCode)
                            {
                                ReturnCode = (short)ErrorCode.Ok,
                                DebugMessage = "lot unpublished"
                            };

                    peer.SendUpdatePlayerInventoryEvent();

                    _auction.SendUpdateAuctionDataToSubscribers();
                }
            }

            return response;
        }
    }
}