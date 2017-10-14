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
                    return @"desc-dev.cloudapp.net:5055";

                case ServerHosts.NewDevelop:
                    return @"138.201.152.100:5055";
                default:
                    throw new ArgumentOutOfRangeException("serverHost", serverHost, null);
            }
        }
    }
}
