namespace Warsmiths.Client.Loadbalancing.Codes
{
    /// <summary>
    /// These (byte) values are for "well known" room/game properties used in Photon Loadbalancing.
    /// </summary>
    /// <remarks>
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public class GamePropertyKey
    {
        /// <summary>
        /// (255) Max number of players that "fit" into this room. 0 is for "unlimited".
        /// </summary>
        public const byte MaxPlayers = 255;

        /// <summary>
        /// (254) Makes this room listed or not in the lobby on master.
        /// </summary>
        public const byte IsVisible = 254;

        /// <summary>
        /// (253) Allows more players to join a room (or not).
        /// </summary>
        public const byte IsOpen = 253;

        /// <summary>
        /// (252) Current count od players in the room. Used only in the lobby on master.
        /// </summary>
        public const byte PlayerCount = 252;

        /// <summary>
        /// (251) True if the room is to be removed from room listing (used in update to room list in lobby on master)
        /// </summary>
        public const byte Removed = 251;

        /// <summary>
        /// (250) A list of the room properties to pass to the RoomInfo list in a lobby. 
        /// This is used in CreateRoom, which defines this list once per room.
        /// </summary>
        public const byte PropsListedInLobby = 250;

        /// <summary>
        /// (249) Equivalent of Operation Join parameter CleanupCacheOnLeave.
        /// </summary>
        public const byte CleanupCacheOnLeave = 249;

        /// <summary>
        /// (248) Code for MasterClientId, which is synced by server. 
        /// When sent as op-parameter this is (byte)203. As room property this is (byte)248.
        /// </summary>
        public const byte MasterClientId = (byte) 248;
    }
}