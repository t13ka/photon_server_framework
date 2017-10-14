namespace YourGame.Client.Loadbalancing.Enums
{
    using System;

    /// <summary>
    /// Flags for "types of properties", being used as filter in OpGetProperties.
    /// </summary>
    [Flags]
    public enum PropertyTypeFlag : byte
    {
        /// <summary>(0x00) Flag type for no property type.</summary>
        None = 0x00,

        /// <summary>(0x01) Flag type for game-attached properties.</summary>
        Game = 0x01,

        /// <summary>(0x02) Flag type for actor related propeties.</summary>
        Actor = 0x02,

        /// <summary>(0x01) Flag type for game AND actor properties. Equal to 'Game'</summary>
        GameAndActor = Game | Actor
    }
}