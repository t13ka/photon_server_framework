using System;
using System.Threading;
using ExitGames.Logging;
using Photon.SocketServer.Diagnostics;

namespace Warsmiths.Server.Framework.Diagnostics
{
    public class CounterLogger : IDisposable
    {
        private const int LogIntervalMs = 5000;
        private static readonly ILogger CounterLog = LogManager.GetLogger("PerformanceCounter");
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private Timer _timer;

        private CounterLogger()
        {
        }

        public static CounterLogger Instance { get; } = new CounterLogger();

        public void Dispose()
        {
            var t = _timer;
            if (t != null)
            {
                t.Dispose();
                _timer = null;
            }
        }

        public void Start()
        {
            if (CounterLog.IsDebugEnabled)
            {
                if (Log.IsInfoEnabled)
                {
                    Log.Info("Starting counter logger");
                }

                var callback = new TimerCallback(LogCounter);
                _timer = new Timer(callback, null, 0, LogIntervalMs);
            }
            else
            {
                if (Log.IsInfoEnabled)
                {
                    Log.Info("Counter logger not started.");
                }
            }
        }

        private static void LogCounter(object state)
        {
            CounterLog.InfoFormat(
                "# Sessions = {0:N2}", PhotonCounter.SessionCount.GetNextValue());
            CounterLog.InfoFormat(
                "OperationReceive/s = {0:N2}",
                PhotonCounter.OperationReceivePerSec.GetNextValue());
            CounterLog.InfoFormat(
                "OperationsResponse/s = {0:N2}",
                PhotonCounter.OperationResponsePerSec.GetNextValue());
            CounterLog.InfoFormat(
                "EventsSent/s = {0:N2}",
                PhotonCounter.EventSentPerSec.GetNextValue());
            CounterLog.InfoFormat(
                "Average Execution Time = {0:N2}\n",
                PhotonCounter.AverageOperationExecutionTime.GetNextValue());
        }
    }
}