using System;
using System.Linq;

using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Services.DomainConfig
{
    using YourGame.Common.Domain;
    using YourGame.DatabaseService.Repositories;
    using YourGame.Server.Framework.Services;

    public class ConfigChangeListenerService : IRuntimeService, IDisposable
    {
        private static TimeSpan RefreshTimeSpan => new TimeSpan(0, 0, 0, 5, 0);

        private readonly PoolFiber _fiber;

        private readonly DomainConfigRepository _configRepository = new DomainConfigRepository();

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public static object Locker = new object();

        public ConfigChangeListenerService()
        {
            CheckAndInstallDefaultConfiguration();

            _fiber = new PoolFiber();
            _fiber.Start();
            _fiber.ScheduleOnInterval(Update, 0, Convert.ToInt64(RefreshTimeSpan.TotalMilliseconds));
        }

        private void CheckAndInstallDefaultConfiguration()
        {
            var currentConfig = _configRepository.SearchFor(t => t.Current).FirstOrDefault();
            if (currentConfig == null)
            {
                var domainConfig = new DomainConfiguration(true);

                // create default config
                _configRepository.Create(domainConfig);

                Log.DebugFormat(
                    "<<<< ================ THE NEW DOMAIN CONFIGURATION INSTALLED TO DATABASE ================ >>>");

                MasterApplication.DomainConfiguration = domainConfig;
            }
            else
            {
                Log.DebugFormat(
                    "<<<< ================ THE DOMAIN CONFIGURATION HAS BEEN EXTRACTED FROM DATABASE ================ >>>");

                MasterApplication.DomainConfiguration = currentConfig;
            }
        }

        private void Update()
        {
            try
            {
                var currentConfig = _configRepository.SearchFor(t => t.Current).FirstOrDefault();
                if (currentConfig != null)
                {
                    if (currentConfig.UpdateDateTime != MasterApplication.DomainConfiguration.UpdateDateTime)
                    {
                        lock (Locker)
                        {
                            MasterApplication.DomainConfiguration = currentConfig;
                        }

                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Domain configuration is updated successfully");
                        }
                    }
                }
                else
                {
                    // idle
                }
            }
            catch (Exception exception)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat($"Error updating the confuguration: {exception.Message}");
                }
            }
        }

        public void AddSubscriber(PeerBase peerBase)
        {
            // idle
        }

        public void RemoveSubscriber(PeerBase peerBase)
        {
            // idle
        }

        public void Dispose()
        {
            _fiber.Dispose();
        }
    }
}