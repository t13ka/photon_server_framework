using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.ClassHandle;

namespace Warsmiths.Server.Handlers.ClassHandle
{
    public class AddAbilityHandler : BaseHandler
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.AddAbility;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new AddAbilityRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var currentPlayer = peer.GetCurrentPlayer();
            var character = currentPlayer.GetCurrentCharacter();

            if (character == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = $"character not selected"
                };
            }

            if (character.AddAbility(request.Position, request.AblilityName))
            {
                character.Update();
                _playerRepository.Update(currentPlayer);

                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.Ok,
                    DebugMessage = "ability added"
                };
            }
            else
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = $"not enought points"
                };
            }

            return response;
        }
    }
}