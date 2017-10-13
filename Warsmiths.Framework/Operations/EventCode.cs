namespace Warsmiths.Server.Framework.Operations
{
    internal enum EventCode : byte
    {
        NoCodeSet = 0,

        Join = 255,

        Leave = 254,

        PropertiesChanged = 253
    }
}