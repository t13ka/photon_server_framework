using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Warsmiths.Common;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Handlers.Combats
{
    public class IncreaseMaxAllyForceHandler : BaseHandler
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.IncreaseMaxAllyForce;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new Operation(peer.Protocol, operationRequest);
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
                    ReturnCode = (short)ErrorCode.CharacterNotSelect,
                };
            }

            if (character.MaxAllyForce < 9)
            {
                character.MaxAllyForce++;
                character.Update();
                _playerRepository.Update(currentPlayer);

                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.Ok,
                };
            }
            else
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.MaxAllyForceReached,
                };
            }
            return response;
        }
    }
}