using System;
using System.Net;
using ExitGames.Concurrency.Fibers;
using ExitGames.Diagnostics.Counter;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using YourGame.Server.Common;
using YourGame.Server.GameServer;
using YourGame.Server.LoadShedding.Diagnostics;

namespace YourGame.Server.LoadShedding
{
    using YourGame.Server.GameServer;

    public class WorkloadController
    {
        #region Constructors and Destructors

        public WorkloadController(
            ApplicationBase application, string instanceName, string applicationName, IPEndPoint latencyEndpointTcp,
            IPEndPoint latencyEndpointUdp, byte latencyOperationCode, long updateIntervalInMs)
        {
            this.latencyOperationCode = latencyOperationCode;
            this.updateIntervalInMs = updateIntervalInMs;
            FeedbackLevel = FeedbackLevel.Normal;
            this.application = application;
            this.applicationName = applicationName;

            fiber = new PoolFiber();
            fiber.Start();

            remoteEndPointTcp = latencyEndpointTcp;
            remoteEndPointUdp = latencyEndpointUdp;

            cpuCounter = new AverageCounterReader(AverageHistoryLength, "Processor", "% Processor Time", "_Total");
            if (!cpuCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", cpuCounter.Name);
            }

            businessLogicQueueCounter = new AverageCounterReader(AverageHistoryLength,
                "Photon Socket Server: Threads and Queues", "Business Logic Queue", instanceName);
            if (!businessLogicQueueCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", businessLogicQueueCounter.Name);
            }

            enetQueueCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server: Threads and Queues",
                "ENet Queue", instanceName);
            if (!enetQueueCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", enetQueueCounter.Name);
            }

            // amazon instances do not have counter for network interfaces
            bytesInCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server", "bytes in/sec",
                instanceName);
            if (!bytesInCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", bytesInCounter.Name);
            }

            bytesOutCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server", "bytes out/sec",
                instanceName);
            if (!bytesOutCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", bytesOutCounter.Name);
            }

            enetThreadsProcessingCounter = new AverageCounterReader(AverageHistoryLength,
                "Photon Socket Server: Threads and Queues", "ENet Threads Processing", instanceName);
            if (!enetThreadsProcessingCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", enetThreadsProcessingCounter.Name);
            }

            enetThreadsActiveCounter = new PerformanceCounterReader("Photon Socket Server: Threads and Queues",
                "ENet Threads Active", instanceName);
            if (!enetThreadsActiveCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", enetThreadsActiveCounter.Name);
            }

            timeSpentInServerInCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server: ENet",
                "Time Spent In Server: In (ms)", instanceName);
            if (!timeSpentInServerInCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", timeSpentInServerInCounter.Name);
            }

            timeSpentInServerOutCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server: ENet",
                "Time Spent In Server: Out (ms)", instanceName);
            if (!timeSpentInServerOutCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", timeSpentInServerOutCounter.Name);
            }

            tcpDisconnectsPerSecondCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server: TCP",
                "TCP: Disconnected Peers +/sec", instanceName);
            if (!tcpDisconnectsPerSecondCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", tcpDisconnectsPerSecondCounter.Name);
            }

            tcpClientDisconnectsPerSecondCounter = new AverageCounterReader(AverageHistoryLength,
                "Photon Socket Server: TCP", "TCP: Disconnected Peers (C) +/sec", instanceName);
            if (!tcpClientDisconnectsPerSecondCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", tcpClientDisconnectsPerSecondCounter.Name);
            }


            tcpPeersCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server: TCP", "TCP: Peers",
                instanceName);
            if (!tcpPeersCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", tcpPeersCounter.Name);
            }

            udpDisconnectsPerSecondCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server: UDP",
                "UDP: Disconnected Peers +/sec", instanceName);
            if (!udpDisconnectsPerSecondCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", udpDisconnectsPerSecondCounter.Name);
            }

            udpClientDisconnectsPerSecondCounter = new AverageCounterReader(AverageHistoryLength,
                "Photon Socket Server: UDP", "UDP: Disconnected Peers (C) +/sec", instanceName);
            if (!udpClientDisconnectsPerSecondCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", udpClientDisconnectsPerSecondCounter.Name);
            }

            udpPeersCounter = new AverageCounterReader(AverageHistoryLength, "Photon Socket Server: UDP", "UDP: Peers",
                instanceName);
            if (!udpPeersCounter.InstanceExists)
            {
                log.WarnFormat("Did not find counter {0}", udpPeersCounter.Name);
            }

            feedbackControlSystem = new FeedbackControlSystem(1000, this.application.ApplicationRootPath);
        }

        #endregion

        #region Events

        public event EventHandler FeedbacklevelChanged;

        #endregion

        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private const int AverageHistoryLength = 100;

        private readonly ApplicationBase application;

        private readonly string applicationName;

        private readonly AverageCounterReader businessLogicQueueCounter;

        private readonly AverageCounterReader bytesInCounter;

        private readonly AverageCounterReader bytesOutCounter;

        private readonly AverageCounterReader cpuCounter;

        private readonly AverageCounterReader enetQueueCounter;

        private readonly AverageCounterReader timeSpentInServerInCounter;

        private readonly AverageCounterReader timeSpentInServerOutCounter;

        private readonly AverageCounterReader enetThreadsProcessingCounter;

        private readonly AverageCounterReader tcpPeersCounter;

        private readonly AverageCounterReader tcpDisconnectsPerSecondCounter;

        private readonly AverageCounterReader tcpClientDisconnectsPerSecondCounter;

        private readonly AverageCounterReader udpPeersCounter;

        private readonly AverageCounterReader udpDisconnectsPerSecondCounter;

        private readonly AverageCounterReader udpClientDisconnectsPerSecondCounter;

        private readonly PerformanceCounterReader enetThreadsActiveCounter;

        private readonly IFeedbackControlSystem feedbackControlSystem;

        private readonly PoolFiber fiber;

        private readonly byte latencyOperationCode;

        private readonly IPEndPoint remoteEndPointTcp;

        private readonly IPEndPoint remoteEndPointUdp;

        private readonly long updateIntervalInMs;

        private LatencyMonitor latencyMonitorTcp;

        private LatencyMonitor latencyMonitorUdp;

        private IDisposable timerControl;

        private ServerState serverState = ServerState.Normal;

        #endregion

        #region Properties

        public FeedbackLevel FeedbackLevel { get; private set; }

        public ServerState ServerState
        {
            get { return serverState; }

            set
            {
                if (value != serverState)
                {
                    var oldValue = serverState;
                    serverState = value;
                    Counter.ServerState.RawValue = (long) ServerState;
                    RaiseFeedbacklevelChanged();

                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("ServerState changed: old={0}, new={1}", oldValue, serverState);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void OnLatencyMonitorConnectFailed(IPEndPoint endPoint, int errorCode, string errorMessage)
        {
            log.ErrorFormat("Latency monitor connection to {0} failed with err {1}: {2}, serverId={3}", endPoint,
                errorCode, errorMessage, GameApplication.ServerId);

            if (endPoint.Equals(remoteEndPointTcp))
            {
                fiber.Schedule(StartTcpLatencyMonitor, 1000);
            }
            else if (endPoint.Equals(remoteEndPointUdp))
            {
                fiber.Schedule(StartUdpLatencyMonitor, 1000);
            }
        }

        public void OnLatencyConnectClosed(IPEndPoint endPoint)
        {
            if (endPoint.Equals(remoteEndPointTcp))
            {
                fiber.Schedule(StartTcpLatencyMonitor, 1000);
            }
            else if (endPoint.Equals(remoteEndPointUdp))
            {
                fiber.Schedule(StartUdpLatencyMonitor, 1000);
            }
        }

        public LatencyMonitor OnLatencyMonitorPeerConnected(InitResponse initResponse)
        {
            var monitor = new LatencyMonitor(initResponse.Protocol, initResponse.PhotonPeer, latencyOperationCode,
                AverageHistoryLength, 500, this);
            var peerType = initResponse.PhotonPeer.GetPeerType();

            if (peerType == PeerType.ENetPeer || peerType == PeerType.ENetOutboundPeer)
            {
                latencyMonitorUdp = monitor;
            }
            else
            {
                latencyMonitorTcp = monitor;
            }

            return monitor;
        }

        /// <summary>
        ///     Starts the workload controller with a specified update interval in
        ///     milliseconds.
        /// </summary>
        public void Start()
        {
            if (timerControl == null)
            {
                timerControl = fiber.ScheduleOnInterval(Update, 100, updateIntervalInMs);
            }

            if (GameServerSettings.Default.EnableLatencyMonitor)
            {
                StartTcpLatencyMonitor();
                StartUdpLatencyMonitor();
            }
        }

        public void Stop()
        {
            if (timerControl != null)
            {
                timerControl.Dispose();
            }

            if (latencyMonitorUdp != null)
            {
                latencyMonitorUdp.Dispose();
            }

            if (latencyMonitorTcp != null)
            {
                latencyMonitorTcp.Dispose();
            }
        }

        #endregion

        #region Methods

        private void StartTcpLatencyMonitor()
        {
            // TCP Latency monitor:
            if (remoteEndPointTcp != null)
            {
                if (application.ConnectToServerTcp(
                    remoteEndPointTcp, applicationName, remoteEndPointTcp))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat(
                            "Connecting TCP latency monitor to {0}:{1}, serverId={2}",
                            remoteEndPointTcp.Address,
                            remoteEndPointTcp.Port,
                            GameApplication.ServerId);
                    }
                }
                else
                {
                    log.WarnFormat(
                        "TCP Latency monitor connection refused on {0}:{1}, serverId={2}",
                        remoteEndPointTcp.Address,
                        remoteEndPointTcp.Port,
                        GameApplication.ServerId);
                }
            }
        }

        private void StartUdpLatencyMonitor()
        {
            // UDP Latency monitor: 
            if (remoteEndPointUdp != null)
            {
                if (application.ConnectToServerUdp(
                    remoteEndPointUdp, applicationName, remoteEndPointUdp, 1, null))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat(
                            "Connecting UDP latency monitor to {0}:{1}",
                            remoteEndPointUdp.Address,
                            remoteEndPointUdp.Port);
                    }
                }
                else
                {
                    log.WarnFormat(
                        "UDP Latency monitor connection refused on {0}:{1}",
                        remoteEndPointUdp.Address,
                        remoteEndPointUdp.Port);
                }
            }
        }

        private void Update()
        {
            var oldValue = feedbackControlSystem.Output;

            if (cpuCounter.InstanceExists)
            {
                var cpuUsage = (int) cpuCounter.GetNextAverage();
                Counter.CpuAvg.RawValue = cpuUsage;
                feedbackControlSystem.SetCpuUsage(cpuUsage);
            }

            if (businessLogicQueueCounter.InstanceExists)
            {
                var businessLogicQueue = (int) businessLogicQueueCounter.GetNextAverage();
                Counter.BusinessQueueAvg.RawValue = businessLogicQueue;
                feedbackControlSystem.SetBusinessLogicQueueLength(businessLogicQueue);
            }

            if (enetQueueCounter.InstanceExists)
            {
                var enetQueue = (int) enetQueueCounter.GetNextAverage();
                Counter.EnetQueueAvg.RawValue = enetQueue;
                feedbackControlSystem.SetENetQueueLength(enetQueue);
            }

            if (bytesInCounter.InstanceExists && bytesOutCounter.InstanceExists)
            {
                var bytes = (int) bytesInCounter.GetNextAverage() + (int) bytesOutCounter.GetNextAverage();
                Counter.BytesInAndOutAvg.RawValue = bytes;
                feedbackControlSystem.SetBandwidthUsage(bytes);
            }

            if (enetThreadsProcessingCounter.InstanceExists && enetThreadsActiveCounter.InstanceExists)
            {
                try
                {
                    var enetThreadsProcessingAvg = enetThreadsProcessingCounter.GetNextAverage();
                    var enetThreadsActiveAvg = enetThreadsActiveCounter.GetNextValue();

                    int enetThreadsProcessing;
                    if (enetThreadsActiveAvg > 0)
                    {
                        enetThreadsProcessing = (int) (enetThreadsProcessingAvg/enetThreadsActiveAvg*100);
                    }
                    else
                    {
                        enetThreadsProcessing = 0;
                    }

                    Counter.EnetThreadsProcessingAvg.RawValue = enetThreadsProcessing;
                    feedbackControlSystem.SetEnetThreadsProcessing(enetThreadsProcessing);
                }
                catch (DivideByZeroException)
                {
                    log.WarnFormat("Could not calculate Enet Threads processing quotient: Enet Threads Active is 0");
                }
            }


            if (tcpPeersCounter.InstanceExists && tcpDisconnectsPerSecondCounter.InstanceExists &&
                tcpClientDisconnectsPerSecondCounter.InstanceExists)
            {
                try
                {
                    var tcpDisconnectsTotal = tcpDisconnectsPerSecondCounter.GetNextAverage();
                    var tcpDisconnectsClient = tcpClientDisconnectsPerSecondCounter.GetNextAverage();
                    var tcpDisconnectsWithoutClientDisconnects = tcpDisconnectsTotal - tcpDisconnectsClient;
                    var tcpPeerCount = tcpPeersCounter.GetNextAverage();

                    int tcpDisconnectRate;
                    if (tcpPeerCount > 0)
                    {
                        tcpDisconnectRate = (int) (tcpDisconnectsWithoutClientDisconnects/tcpPeerCount*1000);
                    }
                    else
                    {
                        tcpDisconnectRate = 0;
                    }

                    Counter.TcpDisconnectRateAvg.RawValue = tcpDisconnectRate;

                    //this.feedbackControlSystem.SetEnetThreadsProcessing(enetThreadsProcessing);
                }
                catch (DivideByZeroException)
                {
                    log.WarnFormat("Could not calculate TCP Disconnect Rate: TCP Peers is 0");
                }
            }

            if (udpPeersCounter.InstanceExists && udpDisconnectsPerSecondCounter.InstanceExists &&
                udpClientDisconnectsPerSecondCounter.InstanceExists)
            {
                try
                {
                    var udpDisconnectsTotal = udpDisconnectsPerSecondCounter.GetNextAverage();
                    var udpDisconnectsClient = udpClientDisconnectsPerSecondCounter.GetNextAverage();
                    var udpDisconnectsWithoutClientDisconnects = udpDisconnectsTotal - udpDisconnectsClient;
                    var udpPeerCount = udpPeersCounter.GetNextAverage();

                    int udpDisconnectRate;
                    if (udpPeerCount > 0)
                    {
                        udpDisconnectRate = (int) (udpDisconnectsWithoutClientDisconnects/udpPeerCount*1000);
                    }
                    else
                    {
                        udpDisconnectRate = 0;
                    }

                    Counter.UdpDisconnectRateAvg.RawValue = udpDisconnectRate;

                    feedbackControlSystem.SetDisconnectRateUdp(udpDisconnectRate);
                }
                catch (DivideByZeroException)
                {
                    log.WarnFormat("Could not calculate UDP Disconnect Rate: UDP Peers is 0");
                }
            }

            if (timeSpentInServerInCounter.InstanceExists && timeSpentInServerOutCounter.InstanceExists)
            {
                var timeSpentInServer = (int) timeSpentInServerInCounter.GetNextAverage() +
                                        (int) timeSpentInServerOutCounter.GetNextAverage();
                Counter.TimeInServerInAndOutAvg.RawValue = timeSpentInServer;
                feedbackControlSystem.SetTimeSpentInServer(timeSpentInServer);
            }

            if (latencyMonitorUdp != null)
            {
                var latencyUdpAvg = latencyMonitorUdp.AverageLatencyMs;
                Counter.LatencyUdpAvg.RawValue = latencyUdpAvg;
                feedbackControlSystem.SetLatencyUdp(latencyUdpAvg);
            }

            if (latencyMonitorTcp != null)
            {
                var latencyTcpAvg = latencyMonitorTcp.AverageLatencyMs;
                Counter.LatencyTcpAvg.RawValue = latencyTcpAvg;
                feedbackControlSystem.SetLatencyTcp(latencyTcpAvg);
            }

            FeedbackLevel = feedbackControlSystem.Output;
            Counter.LoadLevel.RawValue = (byte) FeedbackLevel;

            if (oldValue != FeedbackLevel)
            {
                if (log.IsInfoEnabled)
                {
                    log.InfoFormat("FeedbackLevel changed: old={0}, new={1}", oldValue, FeedbackLevel);
                }

                RaiseFeedbacklevelChanged();
            }
        }

        private void RaiseFeedbacklevelChanged()
        {
            var e = FeedbacklevelChanged;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}