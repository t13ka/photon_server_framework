namespace YourGame.Client.Loadbalancing.Enums
{
    using System;

    /// <summary>
    /// Lite - OpRaiseEvent allows you to cache events and automatically send them to joining players in a room.
    /// Events are cached per event code and player: Event 100 (example!) can be stored once per player.
    /// Cached events can be modified, replaced and removed.
    /// </summary>
    /// <remarks>
    /// Caching works only combination with ReceiverGroup options Others and All.
    /// </remarks>
    public enum EventCaching : byte
    {
        /// <summary>Default value (not sent).</summary>
        DoNotCache = 0,

        /// <summary>Will merge this event's keys with those already cached.</summary>
        [Obsolete] MergeCache = 1,

        /// <summary>Replaces the event cache for this eventCode with this event's content.</summary>
        [Obsolete] ReplaceCache = 2,

        /// <summary>Removes this event (by eventCode) from the cache.</summary>
        [Obsolete] RemoveCache = 3,

        /// <summary>Adds an event to the room's cache</summary>
        AddToRoomCache = 4,

        /// <summary>Adds this event to the cache for actor 0 (becoming a "globally owned" event in the cache).</summary>
        AddToRoomCacheGlobal = 5,

        /// <summary>Remove fitting event from the room's cache.</summary>
        RemoveFromRoomCache = 6,

        /// <summary>Removes events of players who already left the room (cleaning up).</summary>
        RemoveFromRoomCacheForActorsLeft = 7,

        /// <summary>Increase the index of the sliced cache.</summary>
        SliceIncreaseIndex = 10,

        /// <summary>Set the index of the sliced cache. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SliceSetIndex = 11,

        /// <summary>Purge cache slice with index. Exactly one slice is removed from cache. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SlicePurgeIndex = 12,

        /// <summary>Purge cache slices with specified index and anything lower than that. You must set RaiseEventOptions.CacheSliceIndex for this.</summary>
        SlicePurgeUpToIndex = 13,
    }
}