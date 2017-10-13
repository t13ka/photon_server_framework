using System.Linq;

using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Characters;
using Warsmiths.Server.Operations.Response.Character;

namespace Warsmiths.Server.Handlers.Characters
{
    public class SelectCharacterHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.SelectCharacter;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new SelectCharacterRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var currentPlayer = peer.GetCurrentPlayer();

            var ch = currentPlayer.Characters.FirstOrDefault(t => t.Name == request.Name);
            if (ch == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                           {
                               ReturnCode =
                                   (short)ErrorCode.OperationFailed,
                               DebugMessage = "Character not found!"
                           };
            }

            var prevCharacterInfo = string.Empty;
            var character = currentPlayer.GetCurrentCharacter();
            if (character != null)
            {
                prevCharacterInfo = $"prev selected character is: {character.Name} ";
            }

            var characterToSelect = currentPlayer.GetCharacterByName(request.Name);
            currentPlayer.ChangeSelectedCharacterTo(characterToSelect.Name);
            _playerRepository.Update(currentPlayer);

            response =
                new OperationResponse(
                    operationRequest.OperationCode,
                    new CharacterSelectResponse { CharacterName = request.Name })
                    {
                        ReturnCode = (short)ErrorCode.Ok,
                        DebugMessage =
                            prevCharacterInfo
                            + $" Current character {ch.Name}."
                    };

            return response;
        }
    }
}