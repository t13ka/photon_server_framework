using System;
using System.Collections.Generic;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;

using YourGame.Server.Events;
using YourGame.Server.MasterServer.GameServer;

namespace YourGame.Server.Services
{
    using YourGame.Common;
    using YourGame.Server.Framework.Services;

    public class ApplicationStatsRuntimeService : IRuntimeService
    {
        private readonly PoolFiber _fiber;

        private readonly Dictionary<IncomingGameServerPeer, GameServerApplicationState> _gameServerStats =
            new Dictionary<IncomingGameServerPeer, GameServerApplicationState>();

        private int _publishIntervalMs = 1000;
        private readonly HashSet<PeerBase> _subscribers = new HashSet<PeerBase>();
        private IDisposable _publishSchedule;

        public ApplicationStatsRuntimeService()
        {
            _publishIntervalMs = 1000;
            _fiber = new PoolFiber();
            _fiber.Start();
        }

        public void ChangePublishInterval(int interval)
        {
            _publishIntervalMs = interval;
        }

        public int GameCount { get; private set; }

        public int PeerCount
        {
            get { return PeerCountMaster + PeerCountGameServer; }
        }

        public int PeerCountGameServer { get; private set; }
        public int PeerCountMaster { get; private set; }

        public void UpdateGameServerStats(IncomingGameServerPeer gameServerPeer, int peerCount, int gameCount)
        {
            _fiber.Enqueue(() => OnUpdateGameServerStats(gameServerPeer, peerCount, gameCount));
        }

        public void AddSubscriber(PeerBase peer)
        {
            _fiber.Enqueue(() => OnAddSubscriber(peer));
            _fiber.Enqueue(() => OnUpdateMasterPeerCount(1));
        }

        public void RemoveSubscriber(PeerBase peer)
        {
            _fiber.Enqueue(() => OnRemoveSubscriber(peer));
            _fiber.Enqueue(() => OnUpdateMasterPeerCount(-1));
        }

        private void OnUpdateMasterPeerCount(int diff)
        {
            PeerCountMaster += diff;
            OnStatsUpdated();
        }

        private void OnUpdateGameServerStats(IncomingGameServerPeer gameServerPeer, int peerCount, int gameCount)
        {
            GameServerApplicationState stats;
            if (_gameServerStats.TryGetValue(gameServerPeer, out stats) == false)
            {
                stats = new GameServerApplicationState();
                _gameServerStats.Add(gameServerPeer, stats);
            }

            var playerDiff = peerCount - stats.PlayerCount;
            var gameDiff = gameCount - stats.GameCount;
            PeerCountGameServer += playerDiff;
            GameCount += gameDiff;
            stats.PlayerCount = peerCount;
            stats.GameCount = gameCount;

            if (playerDiff != 0 || gameDiff != 0)
            {
                OnStatsUpdated();
            }
        }

        private void OnStatsUpdated()
        {
            if (_subscribers.Count == 0)
            {
                return;
            }

            if (_publishSchedule != null)
            {
                return;
            }

            _publishSchedule = _fiber.Schedule(PublishApplicationStats, _publishIntervalMs);
        }

        private void OnAddSubscriber(PeerBase subscriber)
        {
            _subscribers.Add(subscriber);
            var eventData = GetAppStatsEventData();
            subscriber.SendEvent(eventData, new SendParameters {Unreliable = true});
        }

        private void OnRemoveSubscriber(PeerBase subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        private void PublishApplicationStats()
        {
            _publishSchedule.Dispose();
            _publishSchedule = null;

            var eventData = GetAppStatsEventData();
            eventData.SendTo(_subscribers, new SendParameters {Unreliable = true});
        }

        private EventData GetAppStatsEventData()
        {
            var e = new AppStatsEvent
            {
                MasterPeerCount = PeerCountMaster,
                PlayerCount = PeerCountGameServer,
                GameCount = GameCount
            };

            return new EventData((byte) EventCode.AppStats, e);
        }

        private sealed class GameServerApplicationState
        {
            public int GameCount ;
            public int PlayerCount ;
        }
    }
}