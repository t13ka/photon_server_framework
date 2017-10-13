using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ExitGames.Logging;

namespace Warsmiths.Server.Common
{
    public static class PublicIPAddressReader
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static readonly string[] LookupServiceUrls = new string[]
            {
                "http://licensesp.exitgames.com/echoip",
                "http://licensespch.exitgames.com/echoip",
                "http://api-sth01.exip.org/?call=ip",
                "http://api-ams01.exip.org/?call=ip", 
                "http://api-nyc01.exip.org/?call=ip"
            }; 


        public static IPAddress ParsePublicIpAddress(string publicIpAddressFromSettings)
        {
            if (string.IsNullOrEmpty(publicIpAddressFromSettings))
            {
                return LookupPublicIpAddress();
            }

            IPAddress ipAddress; 
            if (IPAddress.TryParse(publicIpAddressFromSettings, out ipAddress))
            {
                return ipAddress;
            }

            var hostEntry = Dns.GetHostEntry(publicIpAddressFromSettings);
            if (hostEntry.AddressList.Length > 0)
            {
                foreach (var entry in hostEntry.AddressList)
                {
                    if (entry.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ipAddress; 
                    }
                }
            }
            
            return LookupPublicIpAddress(); 
        }

        private static IPAddress LookupPublicIpAddress()
        {
            IPAddress result = null;

            if (LookupServiceUrls.Length == 0)
            {
                throw new InvalidOperationException("Could not lookup the public IP address: no Lookup Service URLs are defined.");
            }

            foreach (string url in LookupServiceUrls)
            {
                result = DoLookupPublicIpAddress(url);
                Log.InfoFormat("Public IP address: {0}, lookup at {1}", result, url);
                break;
            }

            if (result == null)
            {
                throw new InvalidOperationException("Could not retrieve the public IP address. Please make sure that internet access is available, or configure a fixed value for the PublicIPAddress in the app.config.");
            }
            
            return result; 
        }

        private static IPAddress DoLookupPublicIpAddress(string lookupServiceUrl)
        {
            WebResponse response = null;    
            Stream stream = null;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(lookupServiceUrl);
                request.ServicePoint.BindIPEndPointDelegate = BindIpEndPointDelegate;
   
                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (stream == null)
                {
                    throw new InvalidOperationException(string.Format("Failed to lookup public ip address at {0}: No web response received", lookupServiceUrl));
                }

                string address;
                using (var reader = new StreamReader(stream))
                {
                    address = reader.ReadToEnd();
                }

                IPAddress result; 
                if (IPAddress.TryParse(address, out result) == false)
                {
                    throw new FormatException(string.Format("Failed to lookup public ip address at {0}: Parse address failed - Response = {1}", lookupServiceUrl, address));
                }

                return result; 
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Failed to lookup public ip address at {0}: {1}", lookupServiceUrl, ex);
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

                if (response != null)
                {
                    response.Close();
                }
            }        
        }

        private static IPEndPoint BindIpEndPointDelegate(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
        {
            IPEndPoint result;
            if (remoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                result = new IPEndPoint(IPAddress.IPv6Any, 0);
            }
            else
            {
                result = new IPEndPoint(IPAddress.Any, 0);
            }
            
            Log.InfoFormat("Public IP lookup - remote endpoint: {0} ({1}), local endpoint: {2} ({3})", remoteEndPoint.Address, remoteEndPoint.AddressFamily, result.Address, result.AddressFamily );
            return result; 
        }

        public static bool IsLocalIpAddress(IPAddress hostIp)
        {
            // get local IP addresses
            var localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            // is localhost
            if (IPAddress.IsLoopback(hostIp)) return true;

            // is local address
            return localIPs.Contains(hostIp);
        }
    }
}