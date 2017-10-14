using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Logging;
using Warsmiths.Server.GameServer;
using Warsmiths.Server.LoadShedding.Configuration;

namespace Warsmiths.Server.LoadShedding
{
    using YourGame.Server.GameServer;

    internal sealed class FeedbackControlSystem : IFeedbackControlSystem, IDisposable
    {
        #region Constructors and Destructors

        public FeedbackControlSystem(int maxCcu, string applicationRootPath)
        {
            this.maxCcu = maxCcu;
            this.applicationRootPath = applicationRootPath;

            Initialize();

            if (!string.IsNullOrEmpty(applicationRootPath) && Directory.Exists(applicationRootPath))
            {
                fileWatcher = new FileSystemWatcher(applicationRootPath, GameServerSettings.Default.WorkloadConfigFile);
                fileWatcher.Changed += ConfigFileChanged;
                fileWatcher.Created += ConfigFileChanged;
                fileWatcher.Deleted += ConfigFileChanged;
                fileWatcher.Renamed += ConfigFileChanged;
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        #endregion

        public void Dispose()
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
            }
        }

        #region Properties

        public FeedbackLevel Output
        {
            get { return controllerCollection.Output; }
        }

        #endregion

        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly int maxCcu;

        private readonly string applicationRootPath;

        private readonly FileSystemWatcher fileWatcher;

        private FeedbackControllerCollection controllerCollection;

        #endregion

        #region Implemented Interfaces

        #region IFeedbackControlSystem

        public void SetBandwidthUsage(int bytes)
        {
            controllerCollection.SetInput(FeedbackName.Bandwidth, bytes);
        }

        public void SetBusinessLogicQueueLength(int businessLogicQueue)
        {
            controllerCollection.SetInput(FeedbackName.BusinessLogicQueueLength, businessLogicQueue);
        }

        public void SetCpuUsage(int cpuUsage)
        {
            controllerCollection.SetInput(FeedbackName.CpuUsage, cpuUsage);
        }

        public void SetENetQueueLength(int enetQueue)
        {
            controllerCollection.SetInput(FeedbackName.ENetQueueLength, enetQueue);
        }

        public void SetLatencyTcp(int averageLatencyMs)
        {
            controllerCollection.SetInput(FeedbackName.LatencyTcp, averageLatencyMs);
        }

        public void SetLatencyUdp(int averageLatencyMs)
        {
            controllerCollection.SetInput(FeedbackName.LatencyUdp, averageLatencyMs);
        }

        public void SetOutOfRotation(bool isOutOfRotation)
        {
            controllerCollection.SetInput(FeedbackName.OutOfRotation, isOutOfRotation ? 1 : 0);
        }

        public void SetPeerCount(int peerCount)
        {
            controllerCollection.SetInput(FeedbackName.PeerCount, peerCount);
        }

        public void SetTimeSpentInServer(int timeSpentInServer)
        {
            controllerCollection.SetInput(FeedbackName.TimeSpentInServer, timeSpentInServer);
        }

        public void SetEnetThreadsProcessing(int enetThreadsProcessing)
        {
            controllerCollection.SetInput(FeedbackName.EnetThreadsProcessing, enetThreadsProcessing);
        }

        public void SetDisconnectRateUdp(int udpDisconnectRate)
        {
            controllerCollection.SetInput(FeedbackName.DisconnectRateUdp, udpDisconnectRate);
        }

        #endregion

        #endregion

        #region Methods

        private static List<FeedbackController> GetNonConfigurableControllers(int maxCcu)
        {
            var peerCountThresholds = maxCcu == 0
                ? new Dictionary<FeedbackLevel, int>()
                : new Dictionary<FeedbackLevel, int>
                {
                    {FeedbackLevel.Lowest, 1},
                    {FeedbackLevel.Low, 2},
                    {FeedbackLevel.Normal, maxCcu/2},
                    {FeedbackLevel.High, maxCcu*8/10},
                    {FeedbackLevel.Highest, maxCcu}
                };
            var peerCountController = new FeedbackController(FeedbackName.PeerCount, peerCountThresholds, 0,
                FeedbackLevel.Lowest);

            return new List<FeedbackController> {peerCountController};
        }

        private void ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            log.InfoFormat("Configuration file for Feedback Control System {0}\\{1} {2}. Reinitializing...", e.FullPath,
                e.Name, e.ChangeType);
            Initialize();
        }

        private void Initialize()
        {
            // CCU, Out-of-Rotation
            var allControllers = GetNonConfigurableControllers(maxCcu);

            // try to load feedback controllers from file: 
            string message;
            FeedbackControlSystemSection section;
            var filename = Path.Combine(applicationRootPath, GameServerSettings.Default.WorkloadConfigFile);

            if (!ConfigurationLoader.TryLoadFromFile(filename, out section, out message))
            {
                log.WarnFormat(
                    "Could not initialize Feedback Control System from configuration: Invalid configuration file {0}. Using default settings... ({1})",
                    filename,
                    message);
            }

            if (section != null)
            {
                // load controllers from config file.);
                foreach (FeedbackControllerElement controllerElement in section.FeedbackControllers)
                {
                    var dict = new Dictionary<FeedbackLevel, int>();
                    foreach (FeedbackLevelElement level in controllerElement.Levels)
                    {
                        dict.Add(level.Level, level.Value);
                    }

                    var controller = new FeedbackController(controllerElement.Name, dict, controllerElement.InitialInput,
                        controllerElement.InitialLevel);

                    allControllers.Add(controller);
                }

                log.InfoFormat("Initialized FeedbackControlSystem with {0} controllers from config file.",
                    section.FeedbackControllers.Count);
            }
            else
            {
                // default settings, in case no config file was found.
                allControllers.AddRange(DefaultConfiguration.GetDefaultControllers());
            }

            controllerCollection = new FeedbackControllerCollection(allControllers.ToArray());
        }

        #endregion
    }
}