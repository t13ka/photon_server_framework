namespace YourGame.Client.Loadbalancing.Enums
{
    /// <summary>Ways a room can be created or joined.</summary>
    public enum JoinType
    {
        /// <summary>This client creates a room, gets into it (no need to join) and can set room properties.</summary>
        CreateRoom,

        /// <summary>The room existed already and we join into it (not setting room properties).</summary>
        JoinRoom,

        /// <summary>Done on Master Server and (if successful) followed by a Join on Game Server.</summary>
        JoinRandomRoom,

        /// <summary>Client is either joining or creating a room. On Master- and Game-Server.</summary>
        JoinOrCreateRoom
    }
}