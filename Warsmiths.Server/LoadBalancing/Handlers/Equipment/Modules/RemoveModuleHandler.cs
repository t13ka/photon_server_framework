using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Equipment.Modules;

namespace Warsmiths.Server.Handlers.Equipment.Modules
{
    public class RemoveModuleHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.RemoveModule;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new RemoveModuleRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var currentPlayer = peer.GetCurrentPlayer();

            // if errors
            if (response != null)
            {
                return response;
            }

            var equipment = currentPlayer.GetCurrentCharacter().Equipment.Armor;

            if (equipment?.ArmorParts == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = $"Armor does not have armor parts"
                };
            }


            if (equipment.RemoveModule(request.ModuleId, out BaseModule takeOffModule).Success)
            {
                
                currentPlayer.TryAddToInventory(takeOffModule);
                _playerRepository.Update(currentPlayer);
                peer.SendUpdatePlayerInventoryEvent();
                peer.SendUpdateEquipmentEvent();

                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.Ok,
                    DebugMessage = "Module has been removed from armor part"
                };
            }
            else
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = "Can't find module"
                };
            }
            _log.Warn(response.DebugMessage);
            return response;
        }
    }
}
