namespace YourGame.Client.Loadbalancing.Codes
{
    using System;

    /// <summary>
    /// Codes for parameters and events used in PhotonLoadbalancingAPI
    /// </summary>
    public class OperationCode
    {
        [Obsolete("Exchanging encrpytion keys is done internally in the lib now. Don't expect this operation-result.")] public const byte ExchangeKeysForEncryption = 250;

        /// <summary>
        /// (255) Code for OpJoin, to get into a room.
        /// </summary>
        public const byte Join = 255;

        /// <summary>
        /// (230) Authenticates this peer and connects to a virtual application
        /// </summary>
        public const byte Authenticate = 230;

        /// <summary>
        /// (229) Joins lobby (on master)
        /// </summary>
        public const byte JoinLobby = 229;

        /// <summary>
        /// (228) Leaves lobby (on master)
        /// </summary>
        public const byte LeaveLobby = 228;

        /// <summary>
        /// (227) Creates a game (or fails if name exists)
        /// </summary>
        public const byte CreateGame = 227;

        /// <summary>
        /// (226) Join game (by name)
        /// </summary>
        public const byte JoinGame = 226;

        /// <summary>
        /// (225) Joins random game (on master)
        /// </summary>
        public const byte JoinRandomGame = 225;

        // public const byte CancelJoinRandom = 224; // obsolete, cause JoinRandom no longer is a "process". now provides result immediately

        /// <summary>
        /// (254) Code for OpLeave, to get out of a room.
        /// </summary>
        public const byte Leave = (byte) 254;

        /// <summary>
        /// (253) Raise event (in a room, for other actors/players)
        /// </summary>
        public const byte RaiseEvent = (byte) 253;

        /// <summary>
        /// (252) Set Properties (of room or actor/player)
        /// </summary>
        public const byte SetProperties = (byte) 252;

        /// <summary>
        /// (251) Get Properties</summary>
        public const byte GetProperties = (byte) 251;

        /// <summary>
        /// (248) Operation code to change interest groups in Rooms (Lite application and extending ones).
        /// </summary>
        public const byte ChangeGroups = (byte) 248;

        /// <summary>
        /// (222) Request the rooms and online status for a list of friends (by name, which should be unique).
        /// </summary>
        public const byte FindFriends = 222;

        /// <summary>
        /// (221) Request statistics about a specific list of lobbies (their user and game count).
        /// </summary>
        public const byte GetLobbyStats = 221;

        /// <summary>
        /// (220) Get list of regional servers from a NameServer.
        /// </summary>
        public const byte GetRegions = 220;

        /// <summary>
        /// (219) WebRpc Operation.
        /// </summary>
        public const byte WebRpc = 219;

        public const byte CustomOp = (byte)Common.OperationCode.CustomOp;
    }
}