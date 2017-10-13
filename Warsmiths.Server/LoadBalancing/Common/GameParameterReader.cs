using System;
using System.Collections;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.Common
{
    /// <summary>
    ///     Provides methods to read build in game properties from a hashtable.
    /// </summary>
    /// <remarks>
    ///     Build in game properties in the load balancing project are stored as byte values.
    ///     Because some protocols used by photon (Flash, WebSockets) does not support byte values
    ///     the properties will also be searched in the hashtable using there int representation.
    ///     If an int representation is found it will be converted to the byte representation of
    ///     the game property.
    /// </remarks>
    public static class GameParameterReader
    {
        public static bool TryReadDefaultParameter(
            Hashtable propertyTable, out byte? maxPlayer, out bool? isOpen, out bool? isVisible, out object[] properties,
            out string debugMessage)
        {
            isOpen = null;
            isVisible = null;
            properties = null;
            debugMessage = null;
            object value;

            if (TryReadByteParameter(propertyTable, GameParameter.MaxPlayer, out maxPlayer, out value) == false)
            {
                debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.MaxPlayer, typeof (byte), value);
                return false;
            }

            if (TryReadBooleanParameter(propertyTable, GameParameter.IsOpen, out isOpen, out value) == false)
            {
                debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.IsOpen, typeof (bool), value);
                return false;
            }

            if (TryReadBooleanParameter(propertyTable, GameParameter.IsVisible, out isVisible, out value) == false)
            {
                debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.IsVisible, typeof (bool), value);
                return false;
            }

            if (TryReadGameParameter(propertyTable, GameParameter.Properties, out value))
            {
                if (value != null && value is object[] == false)
                {
                    debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.Properties, typeof (object[]), value);
                    return false;
                }

                properties = (object[]) value;
            }

            return true;
        }

        public static bool TryReadBooleanParameter(Hashtable hashtable, GameParameter paramter, out bool? result,
            out object value)
        {
            result = null;

            if (!TryReadGameParameter(hashtable, paramter, out value))
            {
                return true;
            }

            if (value is bool)
            {
                result = (bool) value;
                return true;
            }

            return false;
        }

        public static bool TryReadByteParameter(Hashtable hashtable, GameParameter paramter, out byte? result,
            out object value)
        {
            result = null;

            if (!TryReadGameParameter(hashtable, paramter, out value))
            {
                return true;
            }

            if (value is byte)
            {
                result = (byte) value;
                return true;
            }

            if (value is int)
            {
                result = (byte) (int) value;
                hashtable[(byte) paramter] = result;
                return true;
            }

            if (value is double)
            {
                result = (byte) (double) value;
                hashtable[(byte) paramter] = result;
                return true;
            }

            return false;
        }

        private static bool TryReadGameParameter(Hashtable hashtable, GameParameter paramter, out object result)
        {
            var byteKey = (byte) paramter;
            if (hashtable.ContainsKey(byteKey))
            {
                result = hashtable[byteKey];
                return true;
            }

            var intKey = (int) paramter;
            if (hashtable.ContainsKey(intKey))
            {
                result = hashtable[intKey];
                hashtable.Remove(intKey);
                hashtable[byteKey] = result;
                return true;
            }

            result = null;
            return false;
        }

        private static string GetInvalidGamePropertyTypeMessage(GameParameter parameter, Type expectedType, object value)
        {
            return string.Format("Invalid type for property {0}. Expected type {1} but is {2}", parameter, expectedType,
                value == null ? "null" : value.GetType().ToString());
        }
    }
}