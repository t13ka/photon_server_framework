using ExitGames.Logging;
using MongoDB.Bson.Serialization;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.ListContainer;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Equipment;

namespace Warsmiths.Server.Handlers.Equipment
{
    public class SaveReservedFieldsForCharacterHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.SaveReservedFieldsForCharacter;

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new SaveReservedFieldsForCharacterRequest(peer.Protocol, operationRequest);
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
                    DebugMessage = "Character not selected!"
                };
            }

            _playerRepository.Update(currentPlayer);

            response = new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (short) ErrorCode.Ok,
                DebugMessage = "Profile saved"
            };

            return response;
        }
    }
}