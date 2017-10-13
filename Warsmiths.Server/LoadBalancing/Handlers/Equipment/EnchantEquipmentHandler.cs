using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Equipment;
using Warsmiths.Server.Operations.Response.Inventory;

namespace Warsmiths.Server.Handlers.Equipment
{
    internal class EnchantEquipmentHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.EnchantEquipment;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new EnchantEquipmentRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                _log.Warn("Enchat Fail: Not valid request");
                return response;
            }

            var currentPlayer = peer.GetCurrentPlayer();
            var character = currentPlayer.GetCurrentCharacter();

            if (character == null)
            {
                _log.Warn("Enchat Fail: character not selected");
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = "current character not selected!"
                };
            }

            IEntity entity;
            if (currentPlayer.Inventory.TryGetValue(request.EquipmentId, out entity) == false)
            {
                _log.Warn("Enchat PreFail: Eq not found in inventory. Try find in eqip");
                if (character.Equipment.RightWeapon != null && character.Equipment.RightWeapon._id == request.EquipmentId) entity = character.Equipment.RightWeapon;
                    else if (character.Equipment.LeftWeapon != null && character.Equipment.LeftWeapon._id == request.EquipmentId) entity = character.Equipment.LeftWeapon;

                if (entity == null)
                {
                    _log.Warn("Enchat Fail: Eq not found in eqip.. Epic fail");
                    return new OperationResponse(operationRequest.OperationCode,
                        new InventoryResponse {EntityId = request.EquipmentId})
                    {
                        ReturnCode = (short) ErrorCode.OperationFailed,
                        DebugMessage = $"equipment with id '{request.EquipmentId}' not found!"
                    };
                }
            }

            var equipment = entity as BaseWeapon;

            IEntity spentEnity;


            if (currentPlayer.Inventory.TryGetValue(request.ElementId, out spentEnity) && equipment != null)
            {
                var spentElement = (BaseElement) spentEnity;

                var left = spentElement.Quantity - request.EnchantValue;

                if (left >= 0)
                {
                    // remove spent elements
                    spentElement.Quantity = left;

                    if (spentElement.Quantity == 0)
                    {
                        currentPlayer.TryRemoveFromInventory(spentElement._id);
                    }

                    var enchatInfo = equipment.CalcucaleAndGetEnchantInfo(character, spentElement, request.EnchantValue);

                    if (equipment.TryEnchant(enchatInfo.ChanceToBroke))
                    {
                        equipment.Sharpening = enchatInfo.NewEnchatPercent;

                        response = new OperationResponse(operationRequest.OperationCode,
                            new InventoryResponse {EntityId = equipment._id})
                        {
                            ReturnCode = (short) ErrorCode.Ok,
                            DebugMessage = $"equipment with id '{request.EquipmentId}' has been enchanted successfully"
                        };
                    }
                    else
                    {
                        response = new OperationResponse(operationRequest.OperationCode,
                            new InventoryResponse { EntityId = equipment._id })
                        {
                            ReturnCode = (short)ErrorCode.OperationFailed,
                            DebugMessage =
                                $"equipment with id '{request.EquipmentId}' enchanted fail."
                        };
                    }

                    _playerRepository.Update(currentPlayer);
                }
                else
                {
                    response = new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short) ErrorCode.OperationFailed,
                        DebugMessage = "not enough elements"
                    };
                }
            }
            else
            {
                _log.Warn("Enchat Fail: element not found in invenotory: "+request.ElementId);
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = $"element with id {request.ElementId} is not found in inventory"
                };
            }

            peer.SendUpdatePlayerInventoryEvent();
            peer.SendUpdateEquipmentEvent();

            return response;
        }
    }
}