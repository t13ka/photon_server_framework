using System;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;
using YourGame.Server.ServerToServer.Events;

namespace YourGame.Server.GameServer
{
    public class ApplicationStatsPublisher
    {
        private readonly PoolFiber _fiber;
        private readonly int _publishIntervalMilliseconds = 1000;
        private IDisposable _publishStatsSchedule;

        public ApplicationStatsPublisher(int publishIntervalMilliseconds)
        {
            _publishIntervalMilliseconds = publishIntervalMilliseconds;
            _fiber = new PoolFiber();
            _fiber.Start();
        }

        public int PeerCount { get; private set; }
        public int GameCount { get; private set; }

        public void IncrementPeerCount()
        {
            _fiber.Enqueue(() => UpdatePeerCount(1));
        }

        public void DecrementPeerCount()
        {
            _fiber.Enqueue(() => UpdatePeerCount(-1));
        }

        public void IncrementGameCount()
        {
            _fiber.Enqueue(() => UpdateGameCount(1));
        }

        public void DecrementGameCount()
        {
            _fiber.Enqueue(() => UpdateGameCount(-1));
        }

        private void UpdatePeerCount(int diff)
        {
            PeerCount += diff;
            OnStatsUpdated();
        }

        private void UpdateGameCount(int diff)
        {
            GameCount += diff;
            OnStatsUpdated();
        }

        private void OnStatsUpdated()
        {
            if (_publishStatsSchedule != null)
            {
                return;
            }

            _publishStatsSchedule = _fiber.Schedule(PublishStats, _publishIntervalMilliseconds);
        }

        private void PublishStats()
        {
            _publishStatsSchedule = null;
            var e = new UpdateAppStatsEvent {PlayerCount = PeerCount, GameCount = GameCount};
            GameApplication.Instance.MasterPeer.SendEvent(new EventData((byte) ServerEventCode.UpdateAppStats, e),
                new SendParameters());
        }
    }
}