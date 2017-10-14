namespace YourGame.Client.Loadbalancing
{
    using ExitGames.Client.Photon;

    /// <summary>
    /// Wraps up common room properties needed when you create rooms. Read the individual entries for more details.
    /// </summary>
    public class RoomOptions
    {
        /// <summary>
        /// Defines if this room is listed in the lobby. If not, it also is not joined randomly.
        /// </summary>
        /// <remarks>
        /// A room that is not visible will be excluded from the room lists that are sent to the clients in lobbies.
        /// An invisible room can be joined by name but is excluded from random matchmaking.
        ///
        /// Use this to "hide" a room and simulate "private rooms". Players can exchange a roomname and create it
        /// invisble to avoid anyone else joining it.
        /// </remarks>
        public bool IsVisible = true;

        /// <summary>
        /// Defines if this room can be joined at all.
        /// </summary>
        /// <remarks>
        /// If a room is closed, no player can join this. As example this makes sense when 3 of 4 possible players
        /// start their gameplay early and don't want anyone to join during the game.
        /// The room can still be listed in the lobby (set isVisible to control lobby-visibility).
        /// </remarks>
        public bool IsOpen = true;

        /// <summary>
        /// Max number of players that can be in the room at any time. 0 means "no limit".
        /// </summary>
        public byte MaxPlayers;

        /// <summary>
        /// Time To Live (TTL) for an 'actor' in a room. 
        /// If a client disconnects, this actor is inactive first and removed after this timeout. 
        /// In milliseconds.
        /// </summary>
        public int PlayerTtl;

        /// <summary>
        /// Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.
        /// </summary>
        public int EmptyRoomTtl;

        /// <summary>Activates UserId checks on joining - allowing a users to be only once in the room.</summary>
        /// <remarks>
        /// Turnbased rooms should be created with this check turned on! They should also use custom authentication.
        /// Disabled by default for backwards-compatibility.
        /// </remarks>
        public bool CheckUserOnJoin = false;

        /// <summary>Removes a user's events and properties from the room when a user leaves.</summary>
        /// <remarks>
        /// This makes sense when in rooms where players can't place items in the room and just vanish entirely.
        /// When you disable this, the event history can become too long to load if the room stays in use indefinitely.
        /// Default: true. Cleans up the cache and props of leaving users.
        /// </remarks>
        public bool CleanupCacheOnLeave = true;

        /// <summary>The room's custom properties to set. Use string keys!</summary>
        /// <remarks>
        /// Custom room properties are any key-values you need to define the game's setup.
        /// The shorter your keys are, the better.
        /// Example: Map, Mode (could be "m" when used with "Map"), TileSet (could be "t").
        /// </remarks>
        public Hashtable CustomRoomProperties;

        /// <summary>Defines the custom room properties that get listed in the lobby.</summary>
        /// <remarks>
        /// Name the custom room properties that should be available to clients that are in a lobby.
        /// Use with care. Unless a custom property is essential for matchmaking or user info, it should
        /// not be sent to the lobby, which causes traffic and delays for clients in the lobby.
        ///
        /// Default: No custom properties are sent to the lobby.
        /// </remarks>
        public string[] CustomRoomPropertiesForLobby = new string[0];

        /// <summary>Informs the server of the expected plugin setup.</summary>
        /// <remarks>
        /// The operation will fail in case of a plugin missmatch returning error code PluginMismatch 32757(0x7FFF - 10).
        /// Setting string[]{} means the client expects no plugin to be setup.
        /// Note: for backwards compatibility null omits any check.
        /// </remarks>
        public string[] Plugins;

        /// <summary>
        /// Tells the server to skip room events for joining and leaving players.
        /// </summary>
        /// <remarks>
        /// Using this makes the client unaware of the other players in a room.
        /// That can save some traffic if you have some server logic that updates players
        /// but it can also limit the client's usability.
        /// PUN will break, if you use this.
        /// </remarks>
        public bool SuppressRoomEvents;
    }
}