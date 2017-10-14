using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using ExitGames.Concurrency.Fibers;
using ExitGames.Configuration;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;

using log4net;
using log4net.Config;

using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;

using YourGame.Server.LoadShedding;
using YourGame.Server.LoadShedding.Diagnostics;

using LogManager = ExitGames.Logging.LogManager;

namespace YourGame.Server.GameServer
{
    using YourGame.Server.Common;
    using YourGame.Server.Framework;
    using YourGame.Server.Framework.Messages;

    public class GameApplication : ApplicationBase
    {
        #region Constructors and Destructors

        public GameApplication()
        {
            UpdateMasterEndPoint();

            GamingTcpPort = GameServerSettings.Default.GamingTcpPort;
            GamingUdpPort = GameServerSettings.Default.GamingUdpPort;
            GamingWebSocketPort = GameServerSettings.Default.GamingWebSocketPort;

            ConnectRetryIntervalSeconds = GameServerSettings.Default.ConnectReytryInterval;

            _reader = new NodesReader(ApplicationRootPath, CommonSettings.Default.NodesFileName);
        }

        #endregion

        #region Properties

        protected int ConnectRetryIntervalSeconds;

        #endregion

        #region Constants and Fields

        public static readonly Guid ServerId = Guid.NewGuid();

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static GameApplication _instance;

        private static OutgoingMasterServerPeer _masterPeer;

        private readonly NodesReader _reader;

        private PoolFiber _executionFiber;

        private byte _isReconnecting;

        private Timer _masterConnectRetryTimer;

        #endregion

        #region Public Properties

        public new static GameApplication Instance
        {
            get
            {
                return _instance;
            }

            protected set
            {
                Interlocked.Exchange(ref _instance, value);
            }
        }

        public int? GamingTcpPort { get; protected set; }

        public int? GamingUdpPort { get; protected set; }

        public int? GamingWebSocketPort { get; protected set; }

        public IPEndPoint MasterEndPoint { get; protected set; }

        public ApplicationStatsPublisher AppStatsPublisher { get; protected set; }

        public OutgoingMasterServerPeer MasterPeer
        {
            get
            {
                return _masterPeer;
            }

            protected set
            {
                Interlocked.Exchange(ref _masterPeer, value);
            }
        }

        public IPAddress PublicIpAddress { get; protected set; }

        public WorkloadController WorkloadController { get; protected set; }

        #endregion

        #region Public Methods

        public void ConnectToMaster(IPEndPoint endPoint)
        {
            if (Running == false)
            {
                return;
            }

            if (ConnectToServerTcp(endPoint, "Master", endPoint))
            {
                if (Log.IsInfoEnabled)
                {
                    Log.InfoFormat("Connecting to master at {0}, serverId={1}", endPoint, ServerId);
                }
            }
            else
            {
                Log.WarnFormat("master connection refused - is the process shutting down ? {0}", ServerId);
            }
        }

        public void ConnectToMaster()
        {
            if (Running == false)
            {
                return;
            }

            UpdateMasterEndPoint();
            ConnectToMaster(MasterEndPoint);
        }

        public byte GetCurrentNodeId()
        {
            return _reader.ReadCurrentNodeId();
        }

        public void ReconnectToMaster()
        {
            if (Running == false)
            {
                return;
            }

            Thread.VolatileWrite(ref _isReconnecting, 1);
            _masterConnectRetryTimer = new Timer(o => ConnectToMaster(), null, ConnectRetryIntervalSeconds * 1000, 0);
        }

        #endregion

        #region Methods

        private void UpdateMasterEndPoint()
        {
            IPAddress masterAddress;
            if (!IPAddress.TryParse(GameServerSettings.Default.MasterIPAddress, out masterAddress))
            {
                var hostEntry = Dns.GetHostEntry(GameServerSettings.Default.MasterIPAddress);
                if (hostEntry.AddressList == null || hostEntry.AddressList.Length == 0)
                {
                    throw new ConfigurationException(
                        "MasterIPAddress setting is neither an IP nor an DNS entry: "
                        + GameServerSettings.Default.MasterIPAddress);
                }

                masterAddress = hostEntry.AddressList.First(
                    address => address.AddressFamily == AddressFamily.InterNetwork);

                if (masterAddress == null)
                {
                    throw new ConfigurationException(
                        "MasterIPAddress does not resolve to an IPv4 address! Found: " + string.Join(
                            ", ",
                            hostEntry.AddressList.Select(a => a.ToString()).ToArray()));
                }
            }

            var masterPort = GameServerSettings.Default.OutgoingMasterServerPeerPort;
            MasterEndPoint = new IPEndPoint(masterAddress, masterPort);
        }

        /// <summary>
        ///     Sanity check to verify that game states are cleaned up correctly
        /// </summary>
        protected virtual void CheckGames()
        {
            var roomNames = GameCache.Instance.GetRoomNames();

            foreach (var roomName in roomNames)
            {
                Room room;
                GameCache.Instance.TryGetRoomWithoutReference(roomName, out room);
                room.EnqueueMessage(new RoomMessage((byte)GameMessageCodes.CheckGame));
            }
        }

        protected virtual PeerBase CreateGamePeer(InitRequest initRequest)
        {
            return new GameClientPeer(initRequest, this);
        }

        protected virtual OutgoingMasterServerPeer CreateMasterPeer(InitResponse initResponse)
        {
            return new OutgoingMasterServerPeer(initResponse.Protocol, initResponse.PhotonPeer, this);
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("CreatePeer for {0}", initRequest.ApplicationId);
            }

            // Game server latency monitor connects to self
            if (initRequest.ApplicationId == "LatencyMonitor")
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat(
                        "incoming latency peer at {0}:{1} from {2}:{3}, serverId={4}",
                        initRequest.LocalIP,
                        initRequest.LocalPort,
                        initRequest.RemoteIP,
                        initRequest.RemotePort,
                        ServerId);
                }

                return new LatencyPeer(initRequest.Protocol, initRequest.PhotonPeer);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(
                    "incoming game peer at {0}:{1} from {2}:{3}",
                    initRequest.LocalIP,
                    initRequest.LocalPort,
                    initRequest.RemoteIP,
                    initRequest.RemotePort);
            }

            return CreateGamePeer(initRequest);
        }

        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state)
        {
            if (initResponse.ApplicationId == "LatencyMonitor")
            {
                // latency monitor
                var peer = WorkloadController.OnLatencyMonitorPeerConnected(initResponse);
                return peer;
            }

            // master
            Thread.VolatileWrite(ref _isReconnecting, 0);
            return MasterPeer = CreateMasterPeer(initResponse);
        }

        protected virtual void InitLogging()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationRootPath, "log");
            GlobalContext.Properties["LogFileName"] = "GS" + ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(BinaryPath, "log4net.config")));
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state)
        {
            var ipEndPoint = state as IPEndPoint;
            if (ipEndPoint == null)
            {
                Log.ErrorFormat("Unknown connection failed with err {0}: {1}", errorCode, errorMessage);
                return;
            }

            if (ipEndPoint.Equals(MasterEndPoint))
            {
                if (_isReconnecting == 0)
                {
                    Log.ErrorFormat(
                        "Master connection failed with err {0}: {1}, serverId={2}",
                        errorCode,
                        errorMessage,
                        ServerId);
                }
                else if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat(
                        "Master connection failed with err {0}: {1}, serverId={2}",
                        errorCode,
                        errorMessage,
                        ServerId);
                }

                ReconnectToMaster();
                return;
            }

            WorkloadController.OnLatencyMonitorConnectFailed(ipEndPoint, errorCode, errorMessage);
        }

        protected override void OnStopRequested()
        {
            Log.InfoFormat("OnStopRequested: serverid={0}", ServerId);

            if (_masterConnectRetryTimer != null)
            {
                _masterConnectRetryTimer.Dispose();
            }

            if (WorkloadController != null)
            {
                WorkloadController.Stop();
            }

            if (MasterPeer != null)
            {
                MasterPeer.Disconnect();
            }

            base.OnStopRequested();
        }

        protected override void Setup()
        {
            Instance = this;
            InitLogging();

            Log.InfoFormat("Setup: serverId={0}", ServerId);

            Protocol.AllowRawCustomValues = true;

            PublicIpAddress = PublicIPAddressReader.ParsePublicIpAddress(GameServerSettings.Default.PublicIPAddress);

            var isMaster = PublicIPAddressReader.IsLocalIpAddress(MasterEndPoint.Address)
                           || MasterEndPoint.Address.Equals(PublicIpAddress);

            Counter.IsMasterServer.RawValue = isMaster ? 1 : 0;

            SetupFeedbackControlSystem();
            ConnectToMaster();

            if (GameServerSettings.Default.AppStatsPublishInterval > 0)
            {
                AppStatsPublisher = new ApplicationStatsPublisher(GameServerSettings.Default.AppStatsPublishInterval);
            }

            CounterPublisher.DefaultInstance.AddStaticCounterClass(
                typeof(YourGame.Server.Framework.Diagnostics.Counter),
                ApplicationName);
            CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(Counter), ApplicationName);

            _executionFiber = new PoolFiber();
            _executionFiber.Start();
            _executionFiber.ScheduleOnInterval(CheckGames, 60000, 60000);
        }

        protected void SetupFeedbackControlSystem()
        {
            var latencyEndpointTcp = GetLatencyEndpointTcp();
            var latencyEndpointUdp = GetLatencyEndpointUdp();

            WorkloadController = new WorkloadController(
                this,
                PhotonInstanceName,
                "LatencyMonitor",
                latencyEndpointTcp,
                latencyEndpointUdp,
                1,
                1000);

            WorkloadController.Start();
        }

        protected override void TearDown()
        {
            Log.InfoFormat("TearDown: serverId={0}", ServerId);

            if (WorkloadController != null)
            {
                WorkloadController.Stop();
            }

            if (MasterPeer != null)
            {
                MasterPeer.Disconnect();
            }
        }

        private IPEndPoint GetLatencyEndpointTcp()
        {
            if (!GameServerSettings.Default.EnableLatencyMonitor)
            {
                return null;
            }

            IPEndPoint latencyEndpointTcp;
            if (string.IsNullOrEmpty(GameServerSettings.Default.LatencyMonitorAddress))
            {
                if (GamingTcpPort.HasValue == false)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.Error(
                            "Could not start latency monitor because no tcp port is specified in the application configuration.");
                    }

                    return null;
                }

                if (PublicIpAddress == null)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.Error("Could not latency monitor because public ip adress could not be resolved.");
                    }

                    return null;
                }

                var tcpPort = GameServerSettings.Default.RelayPortTcp == 0
                                  ? GamingTcpPort
                                  : GameServerSettings.Default.RelayPortTcp + GetCurrentNodeId() - 1;
                latencyEndpointTcp = new IPEndPoint(PublicIpAddress, tcpPort.Value);
            }
            else
            {
                if (Global.TryParseIpEndpoint(GameServerSettings.Default.LatencyMonitorAddress, out latencyEndpointTcp)
                    == false)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.ErrorFormat(
                            "Could not start latency monitor because an invalid endpoint ({0}) is specified in the application configuration.",
                            GameServerSettings.Default.LatencyMonitorAddress);
                    }

                    return latencyEndpointTcp;
                }
            }

            return latencyEndpointTcp;
        }

        private IPEndPoint GetLatencyEndpointUdp()
        {
            if (!GameServerSettings.Default.EnableLatencyMonitor)
            {
                return null;
            }

            IPEndPoint latencyEndpointUdp;
            if (string.IsNullOrEmpty(GameServerSettings.Default.LatencyMonitorAddressUdp))
            {
                if (GamingUdpPort.HasValue == false)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.Error(
                            "Could not latency monitor because no udp port is specified in the application configuration.");
                    }

                    return null;
                }

                if (PublicIpAddress == null)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.Error("Could not latency monitor because public ip adress could not be resolved.");
                    }

                    return null;
                }

                var udpPort = GameServerSettings.Default.RelayPortUdp == 0
                                  ? GamingUdpPort
                                  : GameServerSettings.Default.RelayPortUdp + GetCurrentNodeId() - 1;
                latencyEndpointUdp = new IPEndPoint(PublicIpAddress, udpPort.Value);
            }
            else
            {
                if (Global.TryParseIpEndpoint(
                        GameServerSettings.Default.LatencyMonitorAddressUdp,
                        out latencyEndpointUdp) == false)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.ErrorFormat(
                            "Coud not start latency monitor because an invalid endpoint ({0}) is specified in the application configuration.",
                            GameServerSettings.Default.LatencyMonitorAddressUdp);
                    }

                    return latencyEndpointUdp;
                }
            }

            return latencyEndpointUdp;
        }

        #endregion
    }
}