using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Server.Framework.Caching;
using Warsmiths.Server.Framework.Common;
using Warsmiths.Server.Framework.Events;
using Warsmiths.Server.Framework.Messages;

namespace Warsmiths.Server.Framework
{
    public class Room : IDisposable
    {
        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///     Releases resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Constants and Fields

        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private IDisposable _removeTimer;
        private readonly RoomCacheBase _roomCache;
        private int _emptyRoomLiveTime;

        #endregion

        #region Constructors and Destructors

        public Room(string name, RoomCacheBase roomCache = null, int emptyRoomLiveTime = 0)
            : this(name, new PoolFiber(), roomCache, emptyRoomLiveTime)
        {
            ExecutionFiber.Start();
        }

        protected Room(string name, PoolFiber executionFiber, RoomCacheBase roomCache, int emptyRoomLiveTime = 0)
        {
            Name = name;
            ExecutionFiber = executionFiber;
            Actors = new ActorCollection();
            Properties = new PropertyBag<object>();
            _roomCache = roomCache;
            _emptyRoomLiveTime = emptyRoomLiveTime;
        }

        ~Room()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        public PoolFiber ExecutionFiber { get; }

        public bool IsDisposed { get; private set; }

        public string Name { get; }

        public virtual int EmptyRoomLiveTime
        {
            get { return _emptyRoomLiveTime; }

            protected set { _emptyRoomLiveTime = value; }
        }

        protected IDisposable RemoveTimer ;

        public PropertyBag<object> Properties { get; }

        protected ActorCollection Actors { get; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Room name: {0}, Actors: {1}, Properties: {2}", Name, Actors.Count,
                Properties == null ? 0 : Properties.Count).AppendLine();

            foreach (var actor in Actors)
            {
                sb.AppendLine(actor.ToString());
            }

            return sb.ToString();
        }

        public virtual bool BeforeRemoveFromCache()
        {
            if (EmptyRoomLiveTime <= 0)
            {
                return true;
            }

            // execute the schedule with the ExecutionFiber so properties
            // are accessed thread safe.
            ExecutionFiber.Enqueue(() => ScheduleRoomRemoval(EmptyRoomLiveTime));
            return false;
        }

        public void EnqueueMessage(IMessage message)
        {
            ExecutionFiber.Enqueue(() => ProcessMessage(message));
        }

        public void EnqueueOperation(PlayerPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            ExecutionFiber.Enqueue(() => ExecuteOperation(peer, operationRequest, sendParameters));
        }

        public IDisposable ScheduleMessage(IMessage message, long timeMs)
        {
            return ExecutionFiber.Schedule(() => ProcessMessage(message), timeMs);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool dispose)
        {
            IsDisposed = true;

            if (dispose)
            {
                ExecutionFiber.Dispose();
                if (_removeTimer != null)
                {
                    _removeTimer.Dispose();
                    _removeTimer = null;
                }
            }
        }

        protected virtual void ExecuteOperation(PlayerPeer peer, OperationRequest operation,
            SendParameters sendParameters)
        {
            Log.Debug("OPP!!!!!!!!!!!!!");
            //_service.HandleRequest(peer, operation);
        }

        protected virtual void ProcessMessage(IMessage message)
        {
        }

        protected void PublishEvent(RoomEventBase e, Actor actor, SendParameters sendParameters)
        {
            var eventData = new EventData(e.Code, e);
            actor.Peer.SendEvent(eventData, sendParameters);
        }

        protected void PublishEvent(RoomEventBase e, IEnumerable<Actor> actorList, SendParameters sendParameters)
        {
            var peers = actorList.Select(actor => actor.Peer);
            var eventData = new EventData(e.Code, e);
            ApplicationBase.Instance.BroadCastEvent(eventData, peers, sendParameters);
        }

        protected void PublishEvent(EventData e, IEnumerable<Actor> actorList, SendParameters sendParameters)
        {
            var peers = actorList.Select(actor => actor.Peer);
            ApplicationBase.Instance.BroadCastEvent(e, peers, sendParameters);
        }

        protected void ScheduleRoomRemoval(int roomLiveTime)
        {
            if (RemoveTimer != null)
            {
                RemoveTimer.Dispose();
                RemoveTimer = null;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Scheduling room romoval: roomName={0}, liveTime={1:N0}", Name, roomLiveTime);
            }

            RemoveTimer = ExecutionFiber.Schedule(TryRemoveRoomFromCache, roomLiveTime);
        }

        protected void TryRemoveRoomFromCache()
        {
            var removed = _roomCache.TryRemoveRoomInstance(this);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Tried to remove room: roomName={0}, removed={1}", Name, removed);
            }
        }

        #endregion
    }
}