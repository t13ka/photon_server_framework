using System;

using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auth;

namespace Warsmiths.Server.Handlers.Auth
{
    using YourGame.Common;
    using YourGame.DatabaseService.Repositories;
    using YourGame.Server.Framework.Handlers;

    public class LogoutHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private static readonly PlayerRepository PlayerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.Logout;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new LogoutRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            string debugMessage;

            if (TryLogOut(peer, out debugMessage))
            {
                response =
                    new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode = (short)ErrorCode.Ok,
                            DebugMessage = debugMessage
                        };
            }
            else
            {
                response =
                    new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode =
                                (short)ErrorCode.OperationFailed,
                            DebugMessage = debugMessage
                        };
            }

            return response;
        }

        public static bool TryLogOut(MasterClientPeer peer, out string debugMessage)
        {
            bool result;
            var currentPlayer = peer.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                currentPlayer.Online = false;
                currentPlayer.LastExitDateTime = DateTime.UtcNow;

                PlayerRepository.Update(currentPlayer);

                debugMessage = $"Player with name '{currentPlayer.FirstName}' are logout from server";
                result = true;
                peer.CurrentPlayerId = null;
            }
            else
            {
                debugMessage = "can't log out when player are not logged in";
                result = false;
            }

            return result;
        }
    }
}