namespace YourGame.Server.Framework.Operations
{
    using System;

    [Flags]
    public enum PropertyType : byte
    {
        None = 0x00,

        Game = 0x01,

        Actor = 0x02,

        GameAndActor = Game | Actor
    }
}