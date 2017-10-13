using System;
using System.Collections;
using Warsmiths.Server.Common;

namespace Warsmiths.Server.MasterServer.ChannelLobby
{
    public class GameChannelKey : IEquatable<GameChannelKey>
    {
        private readonly int _hashcode;
        public readonly Hashtable Properties;

        public GameChannelKey(Hashtable properties)
        {
            Properties = properties;
            _hashcode = DictionaryExtensions.GetHashCode(Properties);
        }

        public bool Equals(GameChannelKey other)
        {
            return DictionaryExtensions.Equals(Properties, other.Properties);
        }

        public override int GetHashCode()
        {
            return _hashcode;
        }
    }
}