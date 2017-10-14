namespace YourGame.Common.Utils
{
    using System;

    public class NetworkUtils
    {
        public static string GetServerConnectionHost(ServerHosts serverHost)
        {
            switch (serverHost)
            {
                case ServerHosts.Local:
                    return @"localhost:5055";

                case ServerHosts.Production:
                    return @"localhost:5055";

                case ServerHosts.NewDevelop:
                    return @"localhost:5055";
                default:
                    throw new ArgumentOutOfRangeException("serverHost", serverHost, null);
            }
        }
    }
}
