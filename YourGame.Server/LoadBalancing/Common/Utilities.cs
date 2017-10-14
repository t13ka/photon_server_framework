using System.Collections;
using System.Globalization;
using System.IO;
using YourGame.Server.Operations;


namespace YourGame.Server.Common
{
    public static class Utilities
    {
        private static readonly string Amf3IsVisblePropertyKey =
            ((byte) GameParameter.IsVisible).ToString(CultureInfo.InvariantCulture);

        private static readonly string Amf3IsOpenPropertyKey =
            ((byte) GameParameter.IsOpen).ToString(CultureInfo.InvariantCulture);

        private static readonly string Amf3MaxPlayerPropertyKey =
            ((byte) GameParameter.MaxPlayer).ToString(CultureInfo.InvariantCulture);

        private static readonly string Amf3PropertiesPropertyKey =
            ((byte) GameParameter.Properties).ToString(CultureInfo.InvariantCulture);

        private static readonly string Amf3PlayerNamePropertyKey =
            ((byte) ActorParameter.PlayerName).ToString(CultureInfo.InvariantCulture);

        public static void ConvertAs3WellKnownPropertyKeys(Hashtable gameProps, Hashtable actorProps)
        {
            // convert game properties
            if (gameProps != null && gameProps.Count > 0)
            {
                // well known property "is visible"
                if (gameProps.ContainsKey(Amf3IsVisblePropertyKey))
                {
                    gameProps[(byte) GameParameter.IsVisible] = gameProps[Amf3IsVisblePropertyKey];
                    gameProps.Remove(Amf3IsVisblePropertyKey);
                }

                // well known property "is open"
                if (gameProps.ContainsKey(Amf3IsOpenPropertyKey))
                {
                    gameProps[(byte) GameParameter.IsOpen] = gameProps[Amf3IsOpenPropertyKey];
                    gameProps.Remove(Amf3IsOpenPropertyKey);
                }

                // well known property "max players"
                if (gameProps.ContainsKey(Amf3MaxPlayerPropertyKey))
                {
                    gameProps[(byte) GameParameter.MaxPlayer] = gameProps[Amf3MaxPlayerPropertyKey];
                    gameProps.Remove(Amf3MaxPlayerPropertyKey);
                }

                // well known property "props listed in lobby"
                if (gameProps.ContainsKey(Amf3PropertiesPropertyKey))
                {
                    gameProps[(byte) GameParameter.Properties] = gameProps[Amf3PropertiesPropertyKey];
                    gameProps.Remove(Amf3PropertiesPropertyKey);
                }
            }

            // convert actor properties (if any)
            if (actorProps != null && actorProps.Count > 0)
            {
                // well known property "is visible"
                if (actorProps.ContainsKey(Amf3PlayerNamePropertyKey))
                {
                    actorProps[(byte) ActorParameter.PlayerName] = actorProps[Amf3PlayerNamePropertyKey];
                    actorProps.Remove(Amf3PlayerNamePropertyKey);
                }
            }
        }

        /// <summary>
        ///     Converts well known properties sent by AS3/Flash clients - from string to byte-keys.
        /// </summary>
        /// <param name="gamePropertyKeys">The game properties list.</param>
        /// <param name="actorPropertyKeys">The actor properties list.</param>
        public static void ConvertAs3WellKnownPropertyKeys(IList gamePropertyKeys, IList actorPropertyKeys)
        {
            // convert game properties
            if (gamePropertyKeys != null && gamePropertyKeys.Count > 0)
            {
                // well known property "is visible"
                if (gamePropertyKeys.Contains(Amf3IsVisblePropertyKey))
                {
                    gamePropertyKeys.Remove(Amf3IsVisblePropertyKey);
                    gamePropertyKeys.Add((byte) GameParameter.IsVisible);
                }

                // well known property "is open"
                if (gamePropertyKeys.Contains(Amf3IsOpenPropertyKey))
                {
                    gamePropertyKeys.Remove(Amf3IsOpenPropertyKey);
                    gamePropertyKeys.Add((byte) GameParameter.IsOpen);
                }

                // well known property "max players"
                if (gamePropertyKeys.Contains(Amf3MaxPlayerPropertyKey))
                {
                    gamePropertyKeys.Remove(Amf3MaxPlayerPropertyKey);
                    gamePropertyKeys.Add((byte) GameParameter.MaxPlayer);
                }

                // well known property "props listed in lobby"
                if (gamePropertyKeys.Contains(Amf3PropertiesPropertyKey))
                {
                    gamePropertyKeys.Remove(Amf3PropertiesPropertyKey);
                    gamePropertyKeys.Add((byte) GameParameter.Properties);
                }
            }

            // convert actor properties (if any)
            if (actorPropertyKeys != null && actorPropertyKeys.Count > 0)
            {
                // well known property "is visible"
                if (actorPropertyKeys.Contains(Amf3PlayerNamePropertyKey))
                {
                    actorPropertyKeys.Remove(Amf3PlayerNamePropertyKey);
                    actorPropertyKeys.Add((byte) ActorParameter.PlayerName);
                }
            }
        }
    }
}