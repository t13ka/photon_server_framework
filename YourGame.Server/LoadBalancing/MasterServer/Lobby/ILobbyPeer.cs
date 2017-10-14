using Photon.SocketServer;

namespace YourGame.Server.MasterServer.Lobby
{
    public interface ILobbyPeer
    {
        NetworkProtocolType NetworkProtocol { get; }

        string UserId { get; }
    }
}