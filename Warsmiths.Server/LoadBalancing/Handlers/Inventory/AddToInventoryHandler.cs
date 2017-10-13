using ExitGames.Logging;
using MongoDB.Bson.Serialization;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Inventory;
using Warsmiths.Server.Operations.Response.Inventory;

namespace Warsmiths.Server.Handlers.Inventory
{
    public class AddToInventoryHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.AddToInventory;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;

            var peer = (MasterClientPeer) peerBase;

            var request = new AddToInventoryRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var equipment = BsonSerializer.Deserialize<BaseEquipment>(request.Entity);

            var currentPlayer = peer.GetCurrentPlayer();

            if (currentPlayer.TryAddToInventory(equipment))
            {
                _playerRepository.Update(currentPlayer);

                response = new OperationResponse(operationRequest.OperationCode,
                    new InventoryResponse {EntityId = equipment._id })
                {
                    ReturnCode = (short) ErrorCode.Ok,
                    DebugMessage = "take inventory data in event"
                };

                peer.SendUpdatePlayerInventoryEvent();
            }
            else
            {
                response = new OperationResponse(operationRequest.OperationCode,
                    new InventoryResponse {EntityId = equipment._id })
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = "this equipment is already in inventory"
                };
            }

            return response;
        }
    }
}