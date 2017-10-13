using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.MasterServer.Lobby;
using Warsmiths.Server.Operations.Request.GamaManagement;

namespace Warsmiths.Server.Handlers.GameManagement
{
    public class CreateGameHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.CreateGame;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer) peerBase;
            var createGameRequest = new CreateGameRequest(peer.Protocol, operationRequest);

            OperationResponse response;
            if (OperationHelper.ValidateOperation(createGameRequest, _log, out response) == false)
            {
                return response;
            }


            if (string.IsNullOrEmpty(createGameRequest.LobbyName) && peer.AppLobby != null)
            {
                peer.AppLobby.EnqueueOperation(peer, operationRequest, sendParameters);
                return null;
            }

            AppLobby lobby;
            if (
                !peer.Application.LobbyFactory.GetOrCreateAppLobby(createGameRequest.LobbyName,
                    (AppLobbyType) createGameRequest.LobbyType, out lobby))
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (short) ErrorCode.OperationDenied,
                    DebugMessage = "Lobby does not exists"
                };
            }

            lobby.EnqueueOperation(peer, operationRequest, sendParameters);
            return null;
        }
    }
}