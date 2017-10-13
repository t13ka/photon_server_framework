namespace Warsmiths.Server.Framework.Operations
{
    public enum ParameterKey : byte
    {
        GameId = 255,

        ActorNr = 254,

        TargetActorNr = 253,

        Actors = 252,

        Properties = 251,

        Broadcast = 250,

        ActorProperties = 249,

        GameProperties = 248,

        Cache = 247,

        ReceiverGroup = 246,

        Data = 245,

        Code = 244,

        Flush = 243,

        DeleteCacheOnLeave = 241,

        Group = 240,

        GroupsForRemove = 239,

        GroupsForAdd = 238,

        SuppressRoomEvents = 237,
    }
}