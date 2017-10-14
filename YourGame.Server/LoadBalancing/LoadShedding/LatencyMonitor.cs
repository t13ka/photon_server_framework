using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;
using YourGame.Server.GameServer;
using YourGame.Server.LoadShedding.Diagnostics;

namespace YourGame.Server.LoadShedding
{
    public sealed class LatencyMonitor : ServerPeerBase, ILatencyMonitor
    {
        #region Constructors and Destructors

        public LatencyMonitor(
            IRpcProtocol protocol, IPhotonPeer nativePeer, byte operationCode, int maxHistoryLength, int intervalMs,
            WorkloadController workloadController)
            : base(protocol, nativePeer)
        {
            this.operationCode = operationCode;
            this.intervalMs = intervalMs;
            this.workloadController = workloadController;
            latencyHistory = new ValueHistory(maxHistoryLength);
            averageLatencyMs = 0;
            lastLatencyMs = 0;

            log.InfoFormat("{1} connection for latency monitoring established (id={0}), serverId={2}", ConnectionId,
                NetworkProtocol, GameApplication.ServerId);

            if (!Stopwatch.IsHighResolution)
            {
                log.InfoFormat("No hires stopwatch!");
            }

            pingTimer = RequestFiber.ScheduleOnInterval(Ping, 0, this.intervalMs);
        }

        #endregion

        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly int intervalMs;

        private readonly ValueHistory latencyHistory;

        private readonly byte operationCode;

        private readonly WorkloadController workloadController;

        private int averageLatencyMs;

        private int lastLatencyMs;

        private IDisposable pingTimer;

        #endregion

        #region Properties

        public int AverageLatencyMs
        {
            get { return averageLatencyMs; }
        }

        public int LastLatencyMs
        {
            get { return lastLatencyMs; }
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (pingTimer != null)
                {
                    pingTimer.Dispose();
                    pingTimer = null;
                }
            }

            base.Dispose(disposing);
        }

        ////protected override void OnConnectFailed(int errorCode, string errorMessage)
        ////{
        ////    log.WarnFormat("Connect Error {0}: {1}", errorCode, errorMessage);
        ////    if (!this.Disposed)
        ////    {
        ////        // wait a second and try again
        ////        this.RequestFiber.Schedule(this.Connect, 1000);
        ////    }
        ////}

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat(
                    "{1} connection for latency monitoring closed (id={0}, reason={2}, detail={3}, serverId={4})",
                    ConnectionId, NetworkProtocol, reasonCode, reasonDetail, GameApplication.ServerId);
            }

            if (pingTimer != null)
            {
                pingTimer.Dispose();
                pingTimer = null;
            }

            if (!Disposed)
            {
                var address = IPAddress.Parse(RemoteIP);
                workloadController.OnLatencyConnectClosed(new IPEndPoint(address, RemotePort));
                //this.workloadController.Start();
            }
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            throw new NotSupportedException();
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            throw new NotSupportedException();
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            if (operationResponse.ReturnCode == 0)
            {
                var contract = new LatencyOperation(Protocol, operationResponse.Parameters);
                if (!contract.IsValid)
                {
                    log.Error("LatencyOperation contract error: " + contract.GetErrorMessage());
                    return;
                }

                long now = Environment.TickCount;

                var sentTime = contract.SentTime;
                var latencyTicks = now - sentTime.GetValueOrDefault();
                var latencyTime = TimeSpan.FromTicks(latencyTicks);
                var latencyMs = (int) latencyTime.TotalMilliseconds;

                Interlocked.Exchange(ref lastLatencyMs, latencyMs);
                latencyHistory.Add(latencyMs);
                if (NetworkProtocol == NetworkProtocolType.Udp)
                {
                    Counter.LatencyUdp.RawValue = latencyMs;
                }
                else
                {
                    Counter.LatencyTcp.RawValue = latencyMs;
                }
                var newAverage = (int) latencyHistory.Average();
                if (newAverage == 0)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("LatencyAvg=0!");
                    }
                }

                Interlocked.Exchange(ref averageLatencyMs, newAverage);
            }
            else
            {
                log.ErrorFormat("Received Ping Response with Error {0}: {1}", operationResponse.ReturnCode,
                    operationResponse.DebugMessage);
            }
        }

        private void Ping()
        {
            var contract = new LatencyOperation {SentTime = Environment.TickCount};
            var request = new OperationRequest(operationCode, contract);
            SendOperationRequest(request, new SendParameters());
        }

        #endregion
    }
}