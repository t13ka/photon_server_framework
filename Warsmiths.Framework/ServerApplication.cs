using System.IO;

using ExitGames.Logging;
using ExitGames.Logging.Log4Net;

using log4net;
using log4net.Config;

using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;

using Warsmiths.Server.Framework.Diagnostics;

using LogManager = ExitGames.Logging.LogManager;

namespace Warsmiths.Server.Framework
{
    public class ServerApplication : ApplicationBase
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new PlayerPeer(initRequest.Protocol, initRequest.PhotonPeer);
        }

        protected override void Setup()
        {
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationRootPath, "log");

            // log4net
            var path = Path.Combine(BinaryPath, "log4net.config");
            var file = new FileInfo(path);
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }

            Log.InfoFormat("Created application Instance: type={0}", Instance.GetType());

            Initialize();
        }

        protected void Initialize()
        {
            // counters for the photon dashboard
            CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(Counter), "Lite");

            Protocol.AllowRawCustomValues = true;
        }

        protected override void TearDown()
        {
        }
    }
}