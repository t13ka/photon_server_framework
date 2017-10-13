using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Equipment;
using Warsmiths.Server.Operations.Response.Equipment;

namespace Warsmiths.Server.Handlers.Equipment
{
    public class TryWearEquipmentHandler : BaseHandler
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.TryWearEquipment;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;
            var currentPlayer = peer.GetCurrentPlayer();
            var request = new TryWearEquipmentRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }


            var equipment = currentPlayer.Inventory.FirstOrDefault( x => x.Value._id == request.EquipmentId).Value as BaseEquipment;//(BaseEquipment) MasterApplication.DomainConfiguration.Get(request.EquipmentId);
            if (equipment == null)
            {
                _log.Warn("Eq is not in inventory");
                response = new OperationResponse(operationRequest.OperationCode,
                    new WearEquipmentResponse {EquipmentId = request.EquipmentId})
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                };
            }
            else
            {
                var character = currentPlayer.GetCurrentCharacter();

                var wearResult = character.Equipment.PutOn(equipment, (EquipmentPlaceTypes) request.EquipmentPlace);

                if (wearResult.Success)
                {
                    
                    response = new OperationResponse(operationRequest.OperationCode,
                        new WearEquipmentResponse {EquipmentId = request.EquipmentId})
                    {
                        ReturnCode = (short) ErrorCode.Ok,
                    };

                    if (wearResult.TakeOffEquipments != null)
                    {
                        foreach (var item in wearResult.TakeOffEquipments)
                        {
                            currentPlayer.TryAddToInventory(item);
                        }
                    }
                    

                    currentPlayer.TryRemoveFromInventory(equipment._id);
                    character.Update();
                    _playerRepository.Update(currentPlayer);

                    
                    peer.SendUpdatePlayerInventoryEvent();
                    peer.SendUpdateEquipmentEvent();
                }
                else
                {
                    response = new OperationResponse(operationRequest.OperationCode,
                        new WearEquipmentResponse {EquipmentId = request.EquipmentId})
                    {
                        ReturnCode = (short) ErrorCode.OperationFailed,
                    };
                }
            }

            return response;
        }
    }
}