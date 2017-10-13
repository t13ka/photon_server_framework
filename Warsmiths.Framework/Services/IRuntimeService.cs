using Photon.SocketServer;

namespace Warsmiths.Server.Framework.Services
{
    public interface IRuntimeService
    {
        void AddSubscriber(PeerBase peerBase);

        void RemoveSubscriber(PeerBase peerBase);
    }
}
