﻿namespace Warsmiths.Server.Framework.Messages
{
    public interface IMessage
    {
        byte Action { get; }

        object Message { get; }
    }
}