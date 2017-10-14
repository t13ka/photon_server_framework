using System;

using Photon.SocketServer;

namespace YourGame.Server.Services.DomainConfig
{
    using YourGame.Server.Framework.Services;

    public class ConfigChangeListenerService : IRuntimeService, IDisposable
    {
        public void AddSubscriber(PeerBase peerBase)
        {
            throw new NotImplementedException();
        }

        public void RemoveSubscriber(PeerBase peerBase)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}