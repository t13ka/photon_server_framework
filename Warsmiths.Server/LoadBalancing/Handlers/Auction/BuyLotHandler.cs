using System;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auction;
using Warsmiths.Server.Services.Auction;

namespace Warsmiths.Server.Handlers.Auction
{
    public class BuyLotHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.TryBuyLot;

        /// <summary>
        /// </summary>
        private readonly AuctionRuntimeService _auction = ServiceManager.Get<AuctionRuntimeService>();

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var request = new BuyLotRequest(peerBase.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var peer = (MasterClientPeer) peerBase;

            var lot = _auction.GetLotByEquipmentId(request.EquipmentId);
            if (lot == null)
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = "Lot not found!"
                };
            }
            else
            {
                if (lot.OwnerId == peer.UserId)
                {
                    response = new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short) ErrorCode.OperationFailed,
                        DebugMessage = "You can not buy your own lot"
                    };
                }
                else
                {
                    var oldOwnerId = lot.OwnerId;
                    var oldOwnerPlayer = _playerRepository.GetById(oldOwnerId);
                    if (oldOwnerPlayer == null)
                    {
                        return new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode = (short) ErrorCode.OperationFailed,
                            DebugMessage = $"Can't find old owner by id {oldOwnerId}!"
                        };
                    }

                    var currentPlayer = peer.GetCurrentPlayer();

                    if (lot.CheckGoldEnough(currentPlayer) == false) // TODO: review this and change if needed
                    {
                        response = new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode = (short) ErrorCode.OperationFailed,
                            DebugMessage = "Not Enough Money"
                        };
                    }
                    else
                    {
                        currentPlayer.Gold = currentPlayer.Gold - lot.Price;

                        lot.Entity._id = Guid.NewGuid().ToString();

                        // devide to 2
                        var eq = (BaseEquipment) lot.Entity;
                        var sh = (float)eq.Sharpening / 2;
                        var r = (int)Math.Round(sh, MidpointRounding.ToEven);
                        eq.Sharpening -= r;

                        if (currentPlayer.TryAddToInventory(lot.Entity))
                        {
                            oldOwnerPlayer.Gold = oldOwnerPlayer.Gold + lot.Price;

                            _auction.Unpublish(lot);

                            _playerRepository.Update(oldOwnerPlayer);
                            _playerRepository.Update(currentPlayer);

                            response = new OperationResponse(operationRequest.OperationCode)
                            {
                                ReturnCode = (short)ErrorCode.Ok,
                                DebugMessage = "ok. sold."
                            };

                            peer.SendUpdatePlayerInventoryEvent();
                            _auction.SendUpdateAuctionDataToSubscribers();
                        }
                        else
                        {
                            response = new OperationResponse(operationRequest.OperationCode)
                            {
                                ReturnCode = (short)ErrorCode.OperationFailed,
                                DebugMessage = "Can't add to inventory"
                            };
                        }
                    }
                }
            }

            return response;
        }
    }
}