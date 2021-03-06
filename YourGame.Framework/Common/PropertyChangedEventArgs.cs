﻿namespace YourGame.Server.Framework.Common
{
    using System;

    public class PropertyChangedEventArgs<TKey> : EventArgs
    {
        public PropertyChangedEventArgs(TKey key, object value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; private set; }

        public object Value { get; private set; }
    }
}