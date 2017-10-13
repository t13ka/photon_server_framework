using Photon.SocketServer;

namespace Warsmiths.Server.MasterServer.Lobby
{
    public interface ILobbyPeer
    {
        NetworkProtocolType NetworkProtocol { get; }

        string UserId { get; }
    }
}