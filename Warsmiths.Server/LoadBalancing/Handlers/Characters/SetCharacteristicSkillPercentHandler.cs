using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Characters;

namespace Warsmiths.Server.Handlers.Characters
{
    public class SetCharacteristicSkillPercentHandler : BaseHandler
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.SetSkillPercentValue;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new SetSkillPercentValueRequest(peer.Protocol, operationRequest);
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
                               ReturnCode =
                                   (short)ErrorCode.OperationFailed,
                               DebugMessage =
                                   $"character not selected"
                           };
            }

            var characteristic = (CharacteristicE)request.CharacteristicType;

            character.SetCharacteristicSkillPercern(characteristic, request.SkillPercent);
            character.Update();

            _playerRepository.Update(currentPlayer);

            peer.SendUpdateCommonCharacterProfileEvent();

            response =
                new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.Ok,
                        DebugMessage =
                            $"setted. characteristic {characteristic} = "
                            + request.SkillPercent
                    };

            return response;
        }
    }
}