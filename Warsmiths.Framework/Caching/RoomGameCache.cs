namespace Warsmiths.Server.Framework.Caching
{
    public class RoomGameCache : RoomCacheBase
    {
        public static readonly RoomGameCache Instance = new RoomGameCache();

        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new RoomGame(roomId, this);
        }
    }
}