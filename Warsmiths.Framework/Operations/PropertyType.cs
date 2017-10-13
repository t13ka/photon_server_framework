using System;

namespace Warsmiths.Server.Framework.Operations
{
    [Flags]
    public enum PropertyType : byte
    {
        None = 0x00, 
        Game = 0x01, 
        Actor = 0x02, 
        GameAndActor = Game | Actor
    }
}