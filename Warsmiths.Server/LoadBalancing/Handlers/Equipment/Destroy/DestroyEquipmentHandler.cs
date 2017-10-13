using ExitGames.Logging;
using MongoDB.Bson;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Equipment;
using Warsmiths.Server.Operations.Response.Equipment;
using Warsmiths.Server.Services.Economic;

namespace Warsmiths.Server.Handlers.Equipment.Destroy
{
    public class DestroyEquipmentHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly EconomicRuntimeService _economic =
            ServiceManager.Get<EconomicRuntimeService>();

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.DestroyEquipment;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new DestroyEquipmentRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            IEntity entity;

            var currentPlayer = peer.GetCurrentPlayer();

            if (currentPlayer.Inventory.TryGetValue(request.EquipmentId, out entity) == false)
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = $"equipment with id '{request.EquipmentId}' not found!"
                };
            }

            var equipment = (BaseEquipment) entity;
            var destroyResult = _economic.DestroyEquipment(equipment);

            currentPlayer.TryRemoveFromInventory(request.EquipmentId);

            var responseObject = new DestroyEquipmentResponse
            {
                DestroyedEquipmentData = destroyResult.ToBson()
            };

            currentPlayer.TryPackToInventory(destroyResult.Element);

            response = new OperationResponse(operationRequest.OperationCode, responseObject)
            {
                ReturnCode = (short) ErrorCode.Ok,
                DebugMessage = "take inventory data in event"
            };

            _playerRepository.Update(currentPlayer);

            peer.SendUpdatePlayerInventoryEvent();

            return response;
        }
    }
}