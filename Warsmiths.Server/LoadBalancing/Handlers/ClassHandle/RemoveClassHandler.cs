using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.ClassHandle;

namespace Warsmiths.Server.Handlers.ClassHandle
{
    public class RemoveClassHandler : BaseHandler
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.RemoveClass;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new RemoveClassRequest(peer.Protocol, operationRequest);
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

            var classType = (ClassTypes)request.ClassType;

            if (character.Classes.Contains(classType))
            {
                character.Classes.Remove(classType);

                character.Update();

                _playerRepository.Update(currentPlayer);

                return new OperationResponse(operationRequest.OperationCode)
                           {
                               ReturnCode = (short)ErrorCode.Ok,
                               DebugMessage = "removed"
                           };
            }

            response =
                new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.OperationFailed,
                        DebugMessage = $"class not found"
                    };

            return response;
        }
    }
}