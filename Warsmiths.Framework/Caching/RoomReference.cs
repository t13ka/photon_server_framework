using System;

using Photon.SocketServer;

namespace Warsmiths.Server.Framework.Caching
{
    public class RoomReference : IDisposable
    {
        private readonly RoomCacheBase _roomCache;

        public RoomReference(RoomCacheBase roomCache, Room room, PeerBase ownerPeer)
        {
            _roomCache = roomCache;
            Id = Guid.NewGuid();
            Room = room;
            OwnerPeer = ownerPeer;
        }

        public Guid Id { get; }

        public PeerBase OwnerPeer { get; }

        public bool IsDisposed { get; private set; }

        public Room Room { get; protected set; }

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        ~RoomReference()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!IsDisposed)
                {
                    _roomCache.ReleaseRoomReference(this);
                    IsDisposed = true;
                }
            }
        }
    }
}