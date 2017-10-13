using System;
using Warsmiths.Server.LoadShedding;

namespace Warsmiths.Server.MasterServer.GameServer
{
    public class GameServerState : IComparable<GameServerState>
    {
        public GameServerState(FeedbackLevel loadLevel, int peerCount)
        {
            LoadLevel = loadLevel;
            PeerCount = peerCount;
        }

        public FeedbackLevel LoadLevel { get; }

        public int PeerCount { get; }

        public int CompareTo(GameServerState other)
        {
            if (this < other)
            {
                return -1;
            }

            if (this > other)
            {
                return 1;
            }

            return 0;
        }

        public static bool operator >(GameServerState a, GameServerState b)
        {
            if (a.LoadLevel > b.LoadLevel)
            {
                return true;
            }

            return a.PeerCount > b.PeerCount;
        }

        public static bool operator <(GameServerState a, GameServerState b)
        {
            if (a.LoadLevel < b.LoadLevel)
            {
                return true;
            }

            return a.PeerCount < b.PeerCount;
        }
    }
}