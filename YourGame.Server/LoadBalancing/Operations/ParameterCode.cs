namespace YourGame.Server.Operations
{
    using YourGame.Server.Framework.Operations;

    public enum ParameterCode : byte
    {
        // parameters inherited from framework
        GameId = ParameterKey.GameId,
        ActorNr = ParameterKey.ActorNr,
        TargetActorNr = ParameterKey.TargetActorNr,
        Actors = ParameterKey.Actors,
        Properties = ParameterKey.Properties,
        Broadcast = ParameterKey.Broadcast,
        ActorProperties = ParameterKey.ActorProperties,
        GameProperties = ParameterKey.GameProperties,
        Cache = ParameterKey.Cache,
        ReceiverGroup = ParameterKey.ReceiverGroup,
        Data = ParameterKey.Data,
        Code = ParameterKey.Code,
        Flush = ParameterKey.Flush,
        DeleteCacheOnLeave = ParameterKey.DeleteCacheOnLeave,
        Group = ParameterKey.Group,
        GroupsForRemove = ParameterKey.GroupsForRemove,
        GroupsForAdd = ParameterKey.GroupsForAdd,
        SuppressRoomEvents = ParameterKey.SuppressRoomEvents,

        // load balancing project specific parameters
        Address = 230,
        PeerCount = 229,
        GameCount = 228,
        MasterPeerCount = 227,
        UserId = 225,
        ApplicationId = 224,
        Position = 223,
        GameList = 222,
        Secret = 221,
        AppVersion = 220,
        NodeId = 219,
        Info = 218,
        ClientAuthenticationType = 217,
        ClientAuthenticationParams = 216,
        CreateIfNotExists = 215,
        JoinMode = 215,
        ClientAuthenticationData = 214,
        LobbyName = 213,
        LobbyType = 212,

        // Craft

    }
}