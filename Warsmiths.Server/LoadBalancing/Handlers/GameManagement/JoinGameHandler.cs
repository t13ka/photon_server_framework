using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.MasterServer.Lobby;
using Warsmiths.Server.Operations.Request.GamaManagement;

namespace Warsmiths.Server.Handlers.GameManagement
{
    public class JoinGameHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.JoinGame;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer) peerBase;
            var joinGameRequest = new JoinGameRequest(peer.Protocol, operationRequest);

            OperationResponse response;
            if (OperationHelper.ValidateOperation(joinGameRequest, _log, out response) == false)
            {
                return response;
            }

            GameState gameState;
            if (peer.Application.TryGetGame(joinGameRequest.GameId, out gameState))
            {
                gameState.Lobby.EnqueueOperation(peer, operationRequest, sendParameters);
                return null;
            }

            if (joinGameRequest.CreateIfNotExists == false)
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (int)ErrorCode.GameIdNotExists,
                    DebugMessage = "Game does not exist"
                };
            }

            AppLobby lobby;
            if (!peer.Application.LobbyFactory.GetOrCreateAppLobby(joinGameRequest.LobbyName,
                    (AppLobbyType)joinGameRequest.LobbyType, out lobby))
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (int)ErrorCode.OperationDenied,
                    DebugMessage = "Lobby does not exist"
                };
            }

            lobby.EnqueueOperation(peer, operationRequest, sendParameters);
            return null;
        }
    }
}