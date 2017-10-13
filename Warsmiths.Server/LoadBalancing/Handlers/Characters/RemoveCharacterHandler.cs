using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Characters;
using Warsmiths.Server.Operations.Response.Character;

namespace Warsmiths.Server.Handlers.Characters
{
    public class RemoveCharacterHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.RemoveCharacter;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new RemoveCharacterRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                _log.Warn($"character with name '{request.Name}': Invalid remove request.");
                return response;
            }

            var currentPlayer = peer.GetCurrentPlayer();

            var character = currentPlayer.GetCharacterByName(request.Name);
            if (character == null)
            {
                _log.Warn($"character with name '{request.Name}' not found.");

                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = $"character with name '{request.Name}' not found."
                };
            }
            else
            {
                currentPlayer.RemoveCharacter(character);
                _log.Info($"character with name '{request.Name}' removed.");

                response = new OperationResponse(operationRequest.OperationCode,
                    new CharacterRemovedResponse {CharacterName = request.Name})
                {
                    ReturnCode = (short) ErrorCode.Ok,
                    DebugMessage = $"character with name '{request.Name}' removed"
                };

                _playerRepository.Update(currentPlayer);

                peer.SendUpdateCharactersList();
            }

            return response;
        }
    }
}