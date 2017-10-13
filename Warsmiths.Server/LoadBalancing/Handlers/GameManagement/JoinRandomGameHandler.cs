using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.MasterServer.Lobby;
using Warsmiths.Server.Operations.Request.GamaManagement;

namespace Warsmiths.Server.Handlers.GameManagement
{
    public class JoinRandomGameHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.JoinRandomGame;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            var peer = (MasterClientPeer) peerBase;
            var joinRandomGameRequest = new JoinRandomGameRequest(peer.Protocol, operationRequest);

            OperationResponse response;
            if (OperationHelper.ValidateOperation(joinRandomGameRequest, _log, out response) == false)
            {
                return response;
            }

            if (string.IsNullOrEmpty(joinRandomGameRequest.LobbyName) && peer.AppLobby != null)
            {
                peer.AppLobby.EnqueueOperation(peer, operationRequest, sendParameters);
                return null;
            }

            AppLobby lobby;
            if (
                !peer.Application.LobbyFactory.GetOrCreateAppLobby(joinRandomGameRequest.LobbyName,
                    (AppLobbyType) joinRandomGameRequest.LobbyType, out lobby))
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