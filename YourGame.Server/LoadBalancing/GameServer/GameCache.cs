namespace YourGame.Server.GameServer
{
    using YourGame.Server.GameServer;

    using YourGame.Server.Framework;
    using YourGame.Server.Framework.Caching;
    using YourGame.Server.GameServer;

    public class GameCache : RoomCacheBase
    {
        public static readonly GameCache Instance = new GameCache();

        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new Game(roomId, this, GameServerSettings.Default.EmptyRoomLiveTime);
        }
    }
}