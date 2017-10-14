namespace YourGame.Server.Framework.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using ExitGames.Logging;

    using Photon.SocketServer;

    using YourGame.Server.Framework.Diagnostics.OperationLogging;

    public abstract class RoomCacheBase
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected readonly Dictionary<string, RoomInstance> RoomInstances = new Dictionary<string, RoomInstance>();

        protected readonly object SyncRoot = new object();

        public bool TryGetRoomWithoutReference(string roomId, out Room room)
        {
            lock (SyncRoot)
            {
                RoomInstance roomInstance;
                if (!RoomInstances.TryGetValue(roomId, out roomInstance))
                {
                    room = null;
                    return false;
                }

                room = roomInstance.Room;
                return true;
            }
        }

        public RoomReference GetRoomReference(string roomName, PeerBase ownerPeer, params object[] args)
        {
            lock (SyncRoot)
            {
                RoomInstance roomInstance;
                if (!RoomInstances.TryGetValue(roomName, out roomInstance))
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Creating room instance: roomName={0}", roomName);
                    }

                    var room = CreateRoom(roomName, args);
                    roomInstance = new RoomInstance(this, room);
                    RoomInstances.Add(roomName, roomInstance);
                }

                return roomInstance.AddReference(ownerPeer);
            }
        }

        public List<string> GetRoomNames()
        {
            lock (SyncRoot)
            {
                return new List<string>(RoomInstances.Keys);
            }
        }

        public virtual string GetDebugString(string roomName)
        {
            lock (SyncRoot)
            {
                RoomInstance roomInstance;
                if (!RoomInstances.TryGetValue(roomName, out roomInstance))
                {
                    return string.Format("RoomCache: No entry for room name {0}", roomName);
                }

                return string.Format("RoomCache: RoomInstance entry found for room {0}: {1}", roomName, roomInstance);
            }
        }

        public bool TryCreateRoom(
            string roomName,
            PeerBase ownerPeer,
            out RoomReference roomReference,
            params object[] args)
        {
            lock (SyncRoot)
            {
                if (RoomInstances.ContainsKey(roomName))
                {
                    roomReference = null;
                    return false;
                }

                var room = CreateRoom(roomName, args);
                var roomInstance = new RoomInstance(this, room);
                RoomInstances.Add(roomName, roomInstance);
                roomReference = roomInstance.AddReference(ownerPeer);
                return true;
            }
        }

        public bool TryGetRoomReference(string roomId, PeerBase ownerPeer, out RoomReference roomReference)
        {
            lock (SyncRoot)
            {
                RoomInstance roomInstance;
                if (!RoomInstances.TryGetValue(roomId, out roomInstance))
                {
                    roomReference = null;
                    return false;
                }

                roomReference = roomInstance.AddReference(ownerPeer);
                return true;
            }
        }

        public void ReleaseRoomReference(RoomReference roomReference)
        {
            Room room;

            lock (SyncRoot)
            {
                RoomInstance roomInstance;
                if (!RoomInstances.TryGetValue(roomReference.Room.Name, out roomInstance))
                {
                    return;
                }

                roomInstance.ReleaseReference(roomReference);

                // if there are still references to the room left 
                // the room stays into the cache
                if (roomInstance.ReferenceCount > 0)
                {
                    return;
                }

                // ask the room implementation if the room should be 
                // removed automaticly from the cache
                var shouldRemoveRoom = roomInstance.Room.BeforeRemoveFromCache();
                if (shouldRemoveRoom == false)
                {
                    return;
                }

                RoomInstances.Remove(roomInstance.Room.Name);
                room = roomInstance.Room;
            }

            if (room == null)
            {
                // the room hast not been removed from the cache
                return;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Removed room instance: roomId={0}", room.Name);
            }

            room.Dispose();
            OnRoomRemoved(room);
        }

        public bool TryRemoveRoomInstance(Room room)
        {
            RoomInstance roomInstance;

            lock (SyncRoot)
            {
                if (RoomInstances.TryGetValue(room.Name, out roomInstance) == false)
                {
                    return false;
                }

                if (roomInstance.ReferenceCount > 0)
                {
                    return false;
                }

                RoomInstances.Remove(roomInstance.Room.Name);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Removed room instance: roomId={0}", roomInstance.Room.Name);
            }

            roomInstance.Room.Dispose();
            OnRoomRemoved(roomInstance.Room);
            return true;
        }

        protected abstract Room CreateRoom(string roomId, params object[] args);

        protected virtual void OnRoomRemoved(Room room)
        {
        }

        protected class RoomInstance
        {
            private readonly LogQueue _logQueue;

            private readonly Dictionary<Guid, RoomReference> _references;

            private readonly RoomCacheBase _roomFactory;

            public RoomInstance(RoomCacheBase roomFactory, Room room)
            {
                this._roomFactory = roomFactory;
                Room = room;
                _references = new Dictionary<Guid, RoomReference>();
                _logQueue = new LogQueue("RoomInstance " + room.Name, LogQueue.DefaultCapacity);
            }

            public int ReferenceCount
            {
                get
                {
                    return _references.Count;
                }
            }

            public Room Room { get; }

            public RoomReference AddReference(PeerBase ownerPeer)
            {
                var reference = new RoomReference(_roomFactory, Room, ownerPeer);
                _references.Add(reference.Id, reference);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat(
                        "Created room instance reference: roomName={0}, referenceCount={1}",
                        Room.Name,
                        ReferenceCount);
                }

                if (_logQueue.Log.IsDebugEnabled)
                {
                    _logQueue.Add(
                        new LogEntry(
                            "AddReference",
                            string.Format(
                                "RoomName={0}, ReferenceCount={1}, OwnerPeer={2}",
                                Room.Name,
                                ReferenceCount,
                                ownerPeer)));
                }

                return reference;
            }

            public void ReleaseReference(RoomReference reference)
            {
                _references.Remove(reference.Id);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat(
                        "Removed room instance reference: roomName={0}, referenceCount={1}",
                        Room.Name,
                        ReferenceCount);
                }

                if (_logQueue.Log.IsDebugEnabled)
                {
                    _logQueue.Add(
                        new LogEntry(
                            "ReleaseReference",
                            string.Format(
                                "RoomName={0}, ReferenceCount={1}, OwnerPeer={2}",
                                Room.Name,
                                ReferenceCount,
                                reference.OwnerPeer)));
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("RoomInstance for Room {0}: {1} References", Room.Name, ReferenceCount).AppendLine();
                foreach (var reference in _references)
                {
                    sb.AppendFormat(
                        "- Reference ID {0}, hold by Peer {1}",
                        reference.Value.Id,
                        reference.Value.OwnerPeer);
                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }
    }
}