using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Utils;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Equipment;
using Warsmiths.Server.Operations.Response.Equipment;

namespace Warsmiths.Server.Handlers.Equipment
{
    public class TryUnwearEquipmentHandler : BaseHandler
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.TryUnwearEquipment;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new TryUnwearEquipmentRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var equipment = MasterApplication.DomainConfiguration.Get(request.EquipmentId);
            if (equipment == null)
            {
                response = new OperationResponse(operationRequest.OperationCode,
                    new WearEquipmentResponse {EquipmentId = request.EquipmentId})
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = $"equipment with id '{request.EquipmentId}' not found!"
                };
            }
            else
            {
                var currentPlayer = peer.GetCurrentPlayer();

                var unwearedEquipment = currentPlayer.GetCurrentCharacter().Equipment.TakeOff((EquipmentPlaceTypes) request.EquipmentPlace);
                if (unwearedEquipment == null)
                {
                    response = new OperationResponse(operationRequest.OperationCode,
                        new WearEquipmentResponse {EquipmentId = request.EquipmentId})
                    {
                        ReturnCode = (short) ErrorCode.OperationFailed,
                        DebugMessage = "this slot is empty"
                    };
                }
                else
                {
                    response = new OperationResponse(operationRequest.OperationCode,
                        new WearEquipmentResponse {EquipmentId = request.EquipmentId})
                    {
                        ReturnCode = (short) ErrorCode.Ok,
                        DebugMessage = "ok"
                    };

                    currentPlayer.TryAddToInventory(unwearedEquipment);
                    currentPlayer.GetCurrentCharacter().Update();

                    _playerRepository.Update(currentPlayer);

                    
                    peer.SendUpdatePlayerInventoryEvent();
                    peer.SendUpdateEquipmentEvent();
                }
            }

            return response;
        }
    }
}