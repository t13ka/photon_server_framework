namespace YourGame.Server.Framework.Services
{
    using Photon.SocketServer;

    public interface IRuntimeService
    {
        void AddSubscriber(PeerBase peerBase);

        void RemoveSubscriber(PeerBase peerBase);
    }
}
