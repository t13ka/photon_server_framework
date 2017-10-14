using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

using ExitGames.Logging;
using ExitGames.Logging.Log4Net;

using log4net;
using log4net.Config;

using Ninject;

using Photon.SocketServer;

using YourGame.Server.Common;
using YourGame.Server.LoadBalancer;
using YourGame.Server.MasterServer.GameServer;
using YourGame.Server.NinjectConfigModules;
using YourGame.Server.Services;
using YourGame.Server.Services.Auction;
using YourGame.Server.Services.DomainConfig;
using YourGame.Server.Services.Economic;

using LogManager = ExitGames.Logging.LogManager;

namespace YourGame.Server.MasterServer
{
    using YourGame.Common.Domain;
    using YourGame.Common.Domain.Equipment;
    using YourGame.DatabaseService;
    using YourGame.DatabaseService.Repositories;
    using YourGame.Server.Common;
    using YourGame.Server.Framework.Services;
    using YourGame.Server.MasterServer;

    public class MasterApplication : ApplicationBase
    {
        public static DomainConfiguration DomainConfiguration;

        public static List<BaseItem> Items = new List<BaseItem>();

        #region Constants and Fields

        /// <summary>
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private NodesReader _reader;

        #endregion

        #region Properties

        public static ServiceManager ServiceManager { get; private set; }

        public GameServerCollection GameServers { get; protected set; }

        public bool IsMaster => MasterNodeId == LocalNodeId;

        public LoadBalancer<IncomingGameServerPeer> LoadBalancer { get; protected set; }

        protected byte LocalNodeId => _reader.CurrentNodeId;

        protected byte MasterNodeId;

        public GameApplication DefaultApplication { get; private set; }

        #endregion

        #region Public Methods

        public IPAddress GetInternalMasterNodeIpAddress()
        {
            return _reader.GetIpAddress(MasterNodeId);
        }

        public virtual void RemoveGameServerFromLobby(IncomingGameServerPeer gameServerPeer)
        {
            DefaultApplication.OnGameServerRemoved(gameServerPeer);
        }

        #endregion

        #region Methods

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            if (IsGameServerPeer(initRequest))
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Received init request from game server");
                }

                return new IncomingGameServerPeer(initRequest, this);
            }

            if (LocalNodeId == MasterNodeId)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Received init request from game client on leader node");
                }

                return new MasterClientPeer(initRequest, DefaultApplication);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Received init request from game client on slave node");
            }

            return new RedirectedClientPeer(initRequest.Protocol, initRequest.PhotonPeer);
        }

        protected virtual void Initialize()
        {
            BsonMappingConfigurator.Configure();
            GameServers = new GameServerCollection();
            LoadBalancer =
                new LoadBalancer<IncomingGameServerPeer>(Path.Combine(ApplicationRootPath, "LoadBalancer.config"));
            DefaultApplication = new GameApplication("{Default}", LoadBalancer);

            ServiceManager = new ServiceManager();

            // ninject 
            var kernel = new StandardKernel(new EconomicInjectionModule());

            // ------------------ DOMAIN CONFIG UPDATER ----------------------
            ServiceManager.InstallService(kernel.Get<ConfigChangeListenerService>());

            // ----------------------------------------------------------
            Log.Info("Db started");
            ServiceManager.InstallService(kernel.Get<ApplicationStatsRuntimeService>());
            Log.Info("App stat started");
            ServiceManager.InstallService(kernel.Get<EconomicRuntimeService>());
            Log.Info("Economic started");

            kernel = new StandardKernel(new AuctionInjectionModule());
            Log.Info("Auction inject started");

            /*var dd = kernel.Get<AuctionRuntimeService>();
                        Log.Info(dd);*/
            var dd = new AuctionRuntimeService(new LotRepository());
            ServiceManager.InstallService(dd);
            Log.Info("Auction started");

            if (MasterServerSettings.Default.AppStatsPublishInterval > 0)
            {
                ServiceManager.Get<ApplicationStatsRuntimeService>()
                    .ChangePublishInterval(MasterServerSettings.Default.AppStatsPublishInterval);
            }

            InitResolver();
        }

        protected virtual bool IsGameServerPeer(InitRequest initRequest)
        {
            return initRequest.LocalPort == MasterServerSettings.Default.IncomingGameServerPeerPort;
        }

        protected override void OnStopRequested()
        {
            // in case of application restarts, we need to disconnect all GS peers to force them to reconnect. 
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("OnStopRequested... going to disconnect {0} GS peers", GameServers.Count);
            }

            // copy to prevent changes of the underlying enumeration
            if (GameServers != null)
            {
                var gameServers = new Dictionary<string, IncomingGameServerPeer>(GameServers);

                foreach (var gameServer in gameServers)
                {
                    var peer = gameServer.Value;
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Disconnecting GS peer {0}:{1}", peer.RemoteIP, peer.RemotePort);
                    }

                    peer.Disconnect();
                }
            }
        }

        protected override void Setup()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationRootPath, "log");
            GlobalContext.Properties["LogFileName"] = "MS" + ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(BinaryPath, "log4net.config")));

            Protocol.AllowRawCustomValues = true;
            SetUnmanagedDllDirectory();
            Initialize();
        }

        protected override void TearDown()
        {
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        ///     Adds a directory to the search path used to locate the 32-bit or
        ///     64-bit version for unmanaged DLLs used in the application.
        /// </summary>
        /// <remarks>
        ///     Assemblies having references to unmanaged libraries (like
        ///     SqlLite) require either a 32-Bit or a 64-Bit version of the
        ///     library depending on the current process.
        /// </remarks>
        private void SetUnmanagedDllDirectory()
        {
            var unmanagedDllDirectory = Path.Combine(BinaryPath, IntPtr.Size == 8 ? "x64" : "x86");
            var result = SetDllDirectory(unmanagedDllDirectory);

            if (result == false)
            {
                Log.WarnFormat("Failed to set unmanaged dll directory to path {0}", unmanagedDllDirectory);
            }
        }

        private void InitResolver()
        {
            var nodesFileName = CommonSettings.Default.NodesFileName;
            if (string.IsNullOrEmpty(nodesFileName))
            {
                nodesFileName = "Nodes.txt";
            }

            _reader = new NodesReader(ApplicationRootPath, nodesFileName);

            // TODO: remove Proxy code completly
            // if (this.IsResolver && MasterServerSettings.Default.EnableProxyConnections)
            // {
            // // setup for proxy connections
            // this.reader.NodeAdded += this.NodesReader_OnNodeAdded;
            // this.reader.NodeChanged += this.NodesReader_OnNodeChanged;
            // this.reader.NodeRemoved += this.NodesReader_OnNodeRemoved;
            // log.Info("Proxy connections enabled");
            // }
            _reader.Start();

            // use local host id if nodes.txt does not exist or if line ending with 'Y' does not exist, otherwise use fixed node #1
            MasterNodeId = (byte)(LocalNodeId == 0 ? 0 : 1);

            Log.InfoFormat(
                "Current Node (nodeId={0}) is {1}the active master (leader)",
                _reader.CurrentNodeId,
                MasterNodeId == _reader.CurrentNodeId ? string.Empty : "NOT ");
        }

        #endregion
    }
}