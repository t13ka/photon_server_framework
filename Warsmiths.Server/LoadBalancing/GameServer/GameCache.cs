using Warsmiths.Server.Framework;
using Warsmiths.Server.Framework.Caching;

namespace Warsmiths.Server.GameServer
{
    public class GameCache : RoomCacheBase
    {
        public static readonly GameCache Instance = new GameCache();

        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new Game(roomId, this, GameServerSettings.Default.EmptyRoomLiveTime);
        }
    }
}