namespace Warsmiths.Client.Loadbalancing.Codes
{
    /// <summary>
    /// These (byte) values define "well known" properties for an Actor / Player.
    /// </summary>
    /// <remarks>
    /// "Custom properties" have to use a string-type as key. They can be assigned at will.
    /// </remarks>
    public class ActorProperties
    {
        /// <summary>
        /// (255) Name of a player/actor.
        /// </summary>
        public const byte PlayerName = 255; // was: 1

        /// <summary>
        /// (255) Tells you if the player is currently in this game (getting events live).
        /// </summary>
        /// <remarks>
        /// A server-set value for async games, where players can leave the game and return later.
        /// </remarks>
        public const byte IsInactive = 254;
    }
}