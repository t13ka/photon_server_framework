using System.Collections.Generic;
using System.Net;

namespace YourGame.Server.GameServer
{
    public static class Global
    {
        public static readonly Dictionary<string, object> Games = new Dictionary<string, object>();
    
        public static bool TryParseIpEndpoint(string value, out IPEndPoint endPoint)
        {
            endPoint = null;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var parts = value.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            IPAddress address;
            if (IPAddress.TryParse(parts[0], out address) == false)
            {
                return false;
            }

            int port;
            if (int.TryParse(parts[1], out port) == false)
            {
                return false;
            }

            endPoint = new IPEndPoint(address, port);
            return true;
        }
    }
}