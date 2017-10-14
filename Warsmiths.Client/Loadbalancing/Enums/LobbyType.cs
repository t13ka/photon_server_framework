namespace YourGame.Client.Loadbalancing.Enums
{
    /// <summary>
    /// Options of lobby types available. Lobby types might be implemented in certain Photon versions and won't be available on older servers.
    /// </summary>
    public enum LobbyType : byte
    {
        /// <summary>This lobby is used unless another is defined by game or JoinRandom. Room-lists will be sent and JoinRandomRoom can filter by matching properties.</summary>
        Default = 0,

        /// <summary>This lobby type lists rooms like Default but JoinRandom has a parameter for SQL-like "where" clauses for filtering. This allows bigger, less, or and and combinations.</summary>
        SqlLobby = 2,

        /// <summary>This lobby does not send lists of games. It is only used for OpJoinRandomRoom. It keeps rooms available for a while when there are only inactive users left.</summary>
        AsyncRandomLobby = 3
    }
}