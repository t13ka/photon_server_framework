using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;

using Newtonsoft.Json;

using Photon.SocketServer;

using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Domain.Equipment.Purchase;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Services.DomainConfig
{
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

        public List<BaseItem> GetItems()
        {
            var items = new List<BaseItem>
                            {
                                new BaseCraftHero()
                                    {
                                        Name = "hero1",
                                        Health = 1,
                                        Price = 20,
                                        RequiredLevel = 10
                                    },
                                new BaseCraftHero
                                    {
                                        Name = "hero2",
                                        Health = 2,
                                        Price = 20,
                                        RequiredLevel = 0
                                    },
                                new BaseCraftHero
                                    {
                                        Name = "hero3",
                                        Health = 3,
                                        Price = 20,
                                        RequiredLevel = 20
                                    },
                                new BaseCraftHero
                                    {
                                        Name = "hero4",
                                        Health = 4,
                                        Price = 35,
                                        RequiredLevel = 0
                                    },
                                new BaseCraftHero
                                    {
                                        Name = "hero5",
                                        Health = 5,
                                        Price = 20,
                                        RequiredLevel = 40
                                    },
                                new BaseCraftHero
                                    {
                                        Name = "hero6",
                                        Health = 6,
                                        Price = 45,
                                        RequiredLevel = 0
                                    },
                                new BasePerk { Name = "Add", Number = "A1" },
                                new BasePerk { Name = "Move", Number = "B1" },
                                new BasePerk { Name = "Chronograph", Number = "C1", },
                                new BasePerk { Name = "Chip", Number = "D1" },
                                new BasePerk { Name = "BoostsSlots", Number = "E1" },
                                new BasePerk { Name = "Activator", Number = "I1" },
                                new BasePerk { Name = "Lattice", Number = "T1" },
                                new BasePerk { Name = "BuildZone", Number = "Q1" },
                                new BasePerk { Name = "AliancePower", Number = "H1" },
                                new BasePerk { Name = "Power1_3", Number = "D1" },
                                new BasePerk { Name = "Power13_15", Number = "J2" },
                                new BasePerk { Name = "Add2", Number = "A2" },
                                new BasePerk { Name = "Add2_1", Number = "A2" },
                                new BasePerk { Name = "Add3", Number = "A3" },
                                new BasePerk { Name = "Cluster", Number = "1J" },
                                new BasePerk { Name = "Power", Number = "2J" },
                                new BaseCraftMic { Name = "Hand" },
                                new BaseCraftMic { Name = "Coins" },
                                new BaseCraftMic { Name = "Leg" },
                                new BaseCraftMic { Name = "Larec" }
                            };
            return items;
        }

        public List<BaseReciept> GetRecepts()
        {
            const string fn =
                "..\\..\\..\\NewClient\\Assets\\StreamingAssets\\Data\\Craft\\Reciepts\\Admar\\Reciepts.txt";

            if (!File.Exists(fn))
            {
                Log.Error($"{fn} not exists. No reciepts was load... pichal'ka");
                return new List<BaseReciept>();
            }

            var bytes = File.ReadAllText(fn);

            var settins = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };
            var r = JsonConvert.DeserializeObject<List<BaseReciept>>(bytes, settins);

            Log.Info($"Load {r.Count} receipts from {fn}");
            return r;
        }

        private void CheckAndInstallDefaultConfiguration()
        {
            var currentConfig = _configRepository.SearchFor(t => t.Current).FirstOrDefault();
            MasterApplication.Reciepts = GetRecepts();
            MasterApplication.Items = GetItems();

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