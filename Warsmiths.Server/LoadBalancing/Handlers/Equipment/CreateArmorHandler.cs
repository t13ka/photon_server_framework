using System;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Equipment;
using Warsmiths.Server.Operations.Response.Equipment;
using EquipmentFactory = Warsmiths.Server.Factories.EquipmentFactory;

namespace Warsmiths.Server.Handlers.Equipment
{
    public class CreateArmorHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.CreateArmor;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new CreateArmorRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }
            var reciept = request.RecieptData.FromBson<BaseReciept>();

            reciept._id = Guid.NewGuid().ToString();

            var armor = EquipmentFactory.CreateArmor(reciept);

            var currentPlayer = peer.GetCurrentPlayer();

            currentPlayer.TryAddToInventory(armor);

            response = new OperationResponse(operationRequest.OperationCode,
                new CreateArmorResponse {EquipmentId = armor._id, RecieptId = armor.RecieptId })
            {
                ReturnCode = (short) ErrorCode.Ok,
                DebugMessage = $"equipment with id '{armor._id}' has been created!"
            };

            peer.SendUpdatePlayerInventoryEvent();

            _playerRepository.Update(currentPlayer);

            return response;
        }
    }
}