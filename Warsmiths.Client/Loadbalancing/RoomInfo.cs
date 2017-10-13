using ExitGames.Client.Photon;
using Warsmiths.Client.Loadbalancing.Codes;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Warsmiths.Client.Loadbalancing
{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WII || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS3 || UNITY_XBOX360 || UNITY_NACL  || UNITY_FLASH  || UNITY_BLACKBERRY || UNITY_PSP2 || UNITY_WEBGL
#endif

    /// <summary>
    /// Used for Room listings of the lobby (not yet joining). Offers the basic info about a
    /// room: name, player counts, properties, etc.
    /// </summary>
    /// <remarks>
    /// This class resembles info about available rooms, as sent by the Master server's lobby.
    /// Consider all values as readonly. None are synced (only updated by events by server).
    /// </remarks>
    public class RoomInfo
    {
        /// <summary>Used internally in lobby, to mark rooms that are no longer listed (for being full, closed or hidden).</summary>
        protected internal bool removedFromList;

        /// <summary>Backing field for property.</summary>
        private Hashtable customProperties = new Hashtable();

        /// <summary>Backing field for property.</summary>
        protected byte maxPlayers = 0;

        /// <summary>Backing field for property.</summary>
        protected bool isOpen = true;

        /// <summary>Backing field for property.</summary>
        protected bool isVisible = true;

        /// <summary>Backing field for property.</summary>
        protected string name;

        /// <summary>Backing field for property.</summary>
        protected string[] propsListedInLobby;

        /// <summary>Read-only "cache" of custom properties of a room. Set via Room.SetCustomProperties.</summary>
        /// <remarks>All keys are string-typed and the values depend on the game/application.</remarks>
        public Hashtable CustomProperties
        {
            get { return customProperties; }
        }

        /// <summary>The name of a room. Unique identifier for a room/match (per AppId + game-Version).</summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Count of players currently in room. This property is overwritten by the Room class (used when you're in a Room).
        /// </summary>
        public int PlayerCount { get; private set; }

        /// <summary>
        /// State if the local client is already in the game or still going to join it on gameserver (in lobby: false).
        /// </summary>
        public bool IsLocalClientInside ;

        /// <summary>
        /// The limit of players for this room. This property is shown in lobby, too.
        /// If the room is full (players count == maxplayers), joining this room will fail.
        /// </summary>
        /// <remarks>
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public byte MaxPlayers
        {
            get { return maxPlayers; }
        }

        /// <summary>
        /// Defines if the room can be joined.
        /// This does not affect listing in a lobby but joining the room will fail if not open.
        /// If not open, the room is excluded from random matchmaking.
        /// Due to racing conditions, found matches might become closed even while you join them.
        /// Simply re-connect to master and find another.
        /// Use property "IsVisible" to not list the room.
        /// </summary>
        /// <remarks>
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public bool IsOpen
        {
            get { return isOpen; }
        }

        /// <summary>
        /// Defines if the room is listed in its lobby.
        /// Rooms can be created invisible, or changed to invisible.
        /// To change if a room can be joined, use property: open.
        /// </summary>
        /// <remarks>
        /// As part of RoomInfo this can't be set.
        /// As part of a Room (which the player joined), the setter will update the server and all clients.
        /// </remarks>
        public bool IsVisible
        {
            get { return isVisible; }
        }

        /// <summary>
        /// Constructs a RoomInfo to be used in room listings in lobby.
        /// </summary>
        /// <param name="roomName">Name of the room and unique ID at the same time.</param>
        /// <param name="roomProperties">Properties for this room.</param>
        protected internal RoomInfo(string roomName, Hashtable roomProperties)
        {
            CacheProperties(roomProperties);

            name = roomName;
        }

        /// <summary>
        /// Makes RoomInfo comparable (by name).
        /// </summary>
        public override bool Equals(object other)
        {
            var otherRoomInfo = other as RoomInfo;
            return (otherRoomInfo != null && name.Equals(otherRoomInfo.name));
        }

        /// <summary>
        /// Accompanies Equals, using the name's HashCode as return.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <summary>Simple printing method.</summary>
        /// <returns>String showing the RoomInfo.</returns>
        public override string ToString()
        {
            return string.Format("Room: '{0}' visible: {1} open: {2} max: {3} count: {4} customProps: {5}", name,
                isVisible, isOpen, maxPlayers, PlayerCount,
                SupportClass.DictionaryToString(customProperties));
        }

        /// <summary>Copies "well known" properties to fields (isVisible, etc) and caches the custom properties (string-keys only) in a local hashtable.</summary>
        /// <param name="propertiesToCache">New or updated properties to store in this RoomInfo.</param>
        protected internal virtual void CacheProperties(Hashtable propertiesToCache)
        {
            if (propertiesToCache == null || propertiesToCache.Count == 0 ||
                customProperties.Equals(propertiesToCache))
            {
                return;
            }

            // check of this game was removed from the list. in that case, we don't
            // need to read any further properties
            // list updates will remove this game from the game listing
            if (propertiesToCache.ContainsKey(GamePropertyKey.Removed))
            {
                removedFromList = (bool) propertiesToCache[GamePropertyKey.Removed];
                if (removedFromList)
                {
                    return;
                }
            }

            // fetch the "well known" properties of the room, if available
            if (propertiesToCache.ContainsKey(GamePropertyKey.MaxPlayers))
            {
                maxPlayers = (byte) propertiesToCache[GamePropertyKey.MaxPlayers];
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.IsOpen))
            {
                isOpen = (bool) propertiesToCache[GamePropertyKey.IsOpen];
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.IsVisible))
            {
                isVisible = (bool) propertiesToCache[GamePropertyKey.IsVisible];
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.PlayerCount))
            {
                PlayerCount = (int) ((byte) propertiesToCache[GamePropertyKey.PlayerCount]);
            }

            if (propertiesToCache.ContainsKey(GamePropertyKey.PropsListedInLobby))
            {
                propsListedInLobby = propertiesToCache[GamePropertyKey.PropsListedInLobby] as string[];
            }

            // merge the custom properties (from your application) to the cache (only string-typed keys will be kept)
            customProperties.MergeStringKeys(propertiesToCache);
        }
    }
}