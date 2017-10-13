namespace Warsmiths.Server.Framework.Operations
{
    public enum CacheOperation
    {
        DoNotCache = 0, 
        MergeCache = 1, 
        ReplaceCache = 2, 
        RemoveCache = 3,
        AddToRoomCache = 4,
        AddToRoomCacheGlobal = 5,
        RemoveFromRoomCache = 6,
        RemoveFromCacheForActorsLeft = 7
    }
}