using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.MasterServer.Lobby;
using Warsmiths.Server.Operations.Request.GamaManagement;

namespace Warsmiths.Server.Handlers.GameManagement
{
    public class JoinLobbyHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.JoinLobby;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer) peerBase;
            var joinLobbyRequest = new JoinLobbyRequest(peer.Protocol, operationRequest);

            OperationResponse response;
            if (OperationHelper.ValidateOperation(joinLobbyRequest, _log, out response) == false)
            {
                return response;
            }

            // remove peer from the currently joined lobby
            if (peer.AppLobby != null)
            {
                peer.AppLobby.RemovePeer(peer);
                peer.AppLobby = null;
            }

            AppLobby lobby;
            if (
                !peer.Application.LobbyFactory.GetOrCreateAppLobby(joinLobbyRequest.LobbyName,
                    (AppLobbyType) joinLobbyRequest.LobbyType, out lobby))
            {
                return new OperationResponse
                {
                    OperationCode = operationRequest.OperationCode,
                    ReturnCode = (short) ErrorCode.OperationDenied,
                    DebugMessage = "Cannot create lobby"
                };
            }

            peer.AppLobby = lobby;
            peer.AppLobby.JoinLobby(peer, joinLobbyRequest, sendParameters);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Joined lobby: {0}, {1}", joinLobbyRequest.LobbyName, joinLobbyRequest.LobbyType);
            }

            return null;
        }
    }
}