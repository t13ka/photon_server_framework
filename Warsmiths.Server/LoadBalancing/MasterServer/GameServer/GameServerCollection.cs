using System;
using System.Collections.Generic;

namespace Warsmiths.Server.MasterServer.GameServer
{
    public class GameServerCollection : Dictionary<string, IncomingGameServerPeer>
    {
        #region Constants and Fields

        private readonly object _syncRoot = new object();

        #endregion

        #region Public Methods

        public void OnConnect(IncomingGameServerPeer gameServerPeer)
        {
            if (!gameServerPeer.ServerId.HasValue)
            {
                throw new InvalidOperationException("server id cannot be null");
            }

            //Guid id = gameServerPeer.ServerId.Value;
            var key = gameServerPeer.Key;

            lock (_syncRoot)
            {
                IncomingGameServerPeer peer;
                if (TryGetValue(key, out peer))
                {
                    if (gameServerPeer != peer)
                    {
                        peer.Disconnect();
                        peer.RemoveGameServerPeerOnMaster();
                        Remove(key);
                    }
                }

                Add(key, gameServerPeer);
            }
        }

        public void OnDisconnect(IncomingGameServerPeer gameServerPeer)
        {
            if (!gameServerPeer.ServerId.HasValue)
            {
                throw new InvalidOperationException("server id cannot be null");
            }

            //Guid id = gameServerPeer.ServerId.Value;
            var key = gameServerPeer.Key;

            lock (_syncRoot)
            {
                IncomingGameServerPeer peer;
                if (TryGetValue(key, out peer))
                {
                    if (peer == gameServerPeer)
                    {
                        Remove(key);
                    }
                }
            }
        }

        #endregion
    }
}