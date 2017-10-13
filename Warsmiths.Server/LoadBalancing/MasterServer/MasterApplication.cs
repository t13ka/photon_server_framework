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
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Domain.Tasks;
using Warsmiths.Common.Utils;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Common;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.LoadBalancer;
using Warsmiths.Server.MasterServer.GameServer;
using Warsmiths.Server.NinjectConfigModules;
using Warsmiths.Server.Services;
using Warsmiths.Server.Services.Auction;
using Warsmiths.Server.Services.DomainConfig;
using Warsmiths.Server.Services.Economic;
using CraftController = Warsmiths.Server.GameServer.Craft.Field.CraftController;
using LogManager = ExitGames.Logging.LogManager;

namespace Warsmiths.Server.MasterServer
{
    public class MasterApplication : ApplicationBase
    {
        public static DomainConfiguration DomainConfiguration ;

        public static Dictionary<string, CraftController> CraftFields = new Dictionary<string, CraftController>();
        public static List<BaseReciept> Reciepts = new List<BaseReciept>();
        public static List<string> QuestQuene = new List<string>
        {
            "Quest0_1",
            "Quest1_1",
            "Quest2_1",
            "Quest3_1",
            "Quest4_1",
            "Quest5_1"
        };

        public static List<string[]> QuestLevel = new List<string[]>
        {
            new [] { "Quest0_1", "Quest0_2", "Quest0_3" },
            new [] { "Quest1_1", "Quest1_2", "Quest1_3", "Quest1_7", "Quest1_8",  "Quest1_9",  "Quest1_4",  "Quest1_5", "Quest1_6","Quest1_10", "Quest1_11",  "Quest1_12","Quest1_13", "Quest1_14",  "Quest1_15"  },
            new [] { "Quest2_1", "Quest2_2", "Quest2_3", "Quest2_4", "Quest2_5",  "Quest2_6", "Quest2_7", "Quest2_8", "Quest2_9", "Quest2_10", "Quest2_11", "Quest2_12", "Quest2_13", "Quest2_14", "Quest2_15"},
            new [] { "Quest3_1", "Quest3_2", "Quest3_3", "Quest3_4", "Quest3_5",  "Quest3_6", "Quest3_7", "Quest3_8", "Quest3_9","Quest3_10", "Quest3_11", "Quest3_12", "Quest3_13", "Quest3_14", "Quest3_15" },
            new [] { "Quest4_1", "Quest4_2", "Quest4_3", "Quest4_4", "Quest4_5",  "Quest4_6", "Quest4_7", "Quest4_8", "Quest4_9" ,"Quest4_10", "Quest4_11", "Quest4_12" ,"Quest4_13", "Quest4_14", "Quest4_15" },
            new [] { "Quest5_1", "Quest5_2", "Quest5_3" }
        };

        public static List<Task> TaskList = new List<Task>
        {
            new Task
            {
                Position = new CommonClasses.IntPos {X = 900, Y = 100},
                Name = "Landing",
                Status = TaskStatusTypesE.Finished,
                Type = TaskTypesE.Settle,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 850, Y = 230},
                    new CommonClasses.IntPos{X = 850, Y = 180},
                    new CommonClasses.IntPos{X = 815, Y = 270},
                    new CommonClasses.IntPos{X = 760, Y = 300},
                    new CommonClasses.IntPos{X = 710, Y = 290},
                    new CommonClasses.IntPos{X = 670, Y = 270},
                    new CommonClasses.IntPos{X = 622, Y = 245},
                },
                Lvl = 1
            },
            new Task
            {
                Position = new CommonClasses.IntPos {X = 526, Y = 260},
                Type = TaskTypesE.CreateCharacter,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 483, Y = 280},
                    new CommonClasses.IntPos{X = 380, Y = 270},
                    new CommonClasses.IntPos{X = 332, Y = 270},
                    new CommonClasses.IntPos{X = 285, Y = 286},
                    new CommonClasses.IntPos{X = 260, Y = 311},
                    new CommonClasses.IntPos{X = 202, Y = 316},
                    new CommonClasses.IntPos{X = 140, Y = 319},
                },
                Name = "Create character",
                Lvl = 2
            },
            new Task
            {
                Position = new CommonClasses.IntPos {X = 103, Y = 400},
                Type = TaskTypesE.ConquereLand,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 163, Y = 485},
                    new CommonClasses.IntPos{X = 171, Y = 520},
                    new CommonClasses.IntPos{X = 200, Y = 555},
                    new CommonClasses.IntPos{X = 230, Y = 577},
                    new CommonClasses.IntPos{X = 268, Y = 590},
                    new CommonClasses.IntPos{X = 313, Y = 608},
                    new CommonClasses.IntPos{X = 358, Y = 625},
                    new CommonClasses.IntPos{X = 403, Y = 630},
                    new CommonClasses.IntPos{X = 445, Y = 625},
                },
                SecondPath = new List<CommonClasses.IntPos>()
                {
                    new CommonClasses.IntPos{X = 177, Y = 371},
                    new CommonClasses.IntPos{X = 225, Y = 371},
                    new CommonClasses.IntPos{X = 272, Y = 380},
                    new CommonClasses.IntPos{X = 318, Y = 390},
                    new CommonClasses.IntPos{X = 371, Y = 400},
                    new CommonClasses.IntPos{X = 426, Y = 412},
                    new CommonClasses.IntPos{X = 483, Y = 412},
                    new CommonClasses.IntPos{X = 521, Y = 394},
                    new CommonClasses.IntPos{X = 571, Y = 379},
                    new CommonClasses.IntPos{X = 620, Y = 382},
                    new CommonClasses.IntPos{X = 666, Y = 386},
                    new CommonClasses.IntPos{X = 710, Y = 394},
                    new CommonClasses.IntPos{X = 753, Y = 400},
                    new CommonClasses.IntPos{X = 798, Y = 412},
                    new CommonClasses.IntPos{X = 840, Y = 430},
                    new CommonClasses.IntPos{X = 880, Y = 450},
                    new CommonClasses.IntPos{X = 917, Y = 470},
                    new CommonClasses.IntPos{X = 951, Y = 490},
                    new CommonClasses.IntPos{X = 986, Y = 508},
                    new CommonClasses.IntPos{X = 1017, Y = 529},
                },
                Exp = 500,
                Name = "Orbital help request",
                Lvl = 3
            },
            new TaskCraftQuestReciept
            {
                Position = new CommonClasses.IntPos {X = 554, Y = 604},
                Type = TaskTypesE.Craft,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 495, Y = 670},
                    new CommonClasses.IntPos{X = 516, Y = 696},
                    new CommonClasses.IntPos{X = 516, Y = 732},
                    new CommonClasses.IntPos{X = 493, Y = 755},
                    new CommonClasses.IntPos{X = 460, Y = 767},
                },
                RecieptName = "Quest0_3",
                Name = "Craft",
                Lvl = 4,
                Prefix = "a"
            },
            new Task
            {
                Position = new CommonClasses.IntPos {X = 1125, Y = 560},
                Type = TaskTypesE.Harvest,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 970, Y = 547},
                    new CommonClasses.IntPos{X = 922, Y = 528},
                    new CommonClasses.IntPos{X = 878, Y = 516},
                    new CommonClasses.IntPos{X = 841, Y = 502},
                    new CommonClasses.IntPos{X = 787, Y = 554},
                    new CommonClasses.IntPos{X = 830, Y = 604},
                    new CommonClasses.IntPos{X = 867, Y = 648},
                    new CommonClasses.IntPos{X = 854, Y = 697},
                    new CommonClasses.IntPos{X = 831, Y = 746},
                    new CommonClasses.IntPos{X = 794, Y = 791},
                    new CommonClasses.IntPos{X = 539, Y = 911},
                    new CommonClasses.IntPos{X = 507, Y = 925},
                    new CommonClasses.IntPos{X = 473, Y = 907},
                    
                },
                Name = "Mining",
                Lvl = 4,
                Prefix = "b"
            },      new Task
            {
                Position = new CommonClasses.IntPos {X = 457, Y = 831},
                Type = TaskTypesE.KillNpc,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 533, Y = 803},
                    new CommonClasses.IntPos{X = 560, Y = 787},
                    new CommonClasses.IntPos{X = 594, Y = 777},
                    new CommonClasses.IntPos{X = 633, Y = 777},
                    new CommonClasses.IntPos{X = 670, Y = 777},
                    new CommonClasses.IntPos{X = 710, Y = 777},
                    new CommonClasses.IntPos{X = 750, Y = 777},
                    new CommonClasses.IntPos{X = 795, Y = 777},
                    new CommonClasses.IntPos{X = 832, Y = 761},
                    new CommonClasses.IntPos{X = 865, Y = 735},
                    new CommonClasses.IntPos{X = 905, Y = 722},
                    new CommonClasses.IntPos{X = 953, Y = 732},
                    new CommonClasses.IntPos{X = 996, Y = 755},
                    new CommonClasses.IntPos{X = 1040, Y = 781},
                    new CommonClasses.IntPos{X = 1088, Y = 790},
                    new CommonClasses.IntPos{X = 1125, Y = 826},
                    new CommonClasses.IntPos{X = 1139, Y = 871},
                    new CommonClasses.IntPos{X = 1142, Y = 917},
                },
                Name = "The work for survivors",
                Lvl = 5
            },
            new TaskCraftQuestReciept
            {
                Position = new CommonClasses.IntPos {X = 1125, Y = 1015},
                Type = TaskTypesE.Game,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 1025, Y = 1060},
                    new CommonClasses.IntPos{X = 1002, Y = 1103},
                    new CommonClasses.IntPos{X = 964, Y = 1132},
                    new CommonClasses.IntPos{X = 917, Y = 1153},
                    new CommonClasses.IntPos{X = 867, Y = 1162},
                    new CommonClasses.IntPos{X = 809, Y = 1155},
                    new CommonClasses.IntPos{X = 754, Y = 1140},
                    new CommonClasses.IntPos{X = 692, Y = 1127},
                    new CommonClasses.IntPos{X = 867, Y = 1162},
                    new CommonClasses.IntPos{X = 641, Y = 1114},
                    new CommonClasses.IntPos{X = 582, Y = 1103},
                    new CommonClasses.IntPos{X = 526, Y = 1117},
                },
                RecieptName = "Quest2_3",
                Name = "The resistance of the planet",
                Lvl = 6
            },
            new TaskCraftQuestReciept
            {
                Position = new CommonClasses.IntPos {X = 441, Y = 1180},
                Type = TaskTypesE.KillNpc,
                FirstPath = new List<CommonClasses.IntPos>
                {
                    new CommonClasses.IntPos{X = 348, Y = 1235},
                    new CommonClasses.IntPos{X = 365, Y = 1275},
                    new CommonClasses.IntPos{X = 405, Y = 1303},
                    new CommonClasses.IntPos{X = 454, Y = 1325},
                    new CommonClasses.IntPos{X = 500, Y = 1350},
                    new CommonClasses.IntPos{X = 500, Y = 1389},
                    new CommonClasses.IntPos{X = 460, Y = 1408},
                    new CommonClasses.IntPos{X = 437, Y = 1442},
                    new CommonClasses.IntPos{X = 403, Y = 1472},
                    new CommonClasses.IntPos{X = 354, Y = 1481},
                },
                RecieptName = "Quest3_3",
                Name = "The extraction of elements",
                Lvl = 7
            }, 
            new TaskCraftQuestReciept
            {
             Position = new CommonClasses.IntPos {X = 313, Y = 1560},
            Type = TaskTypesE.KillNpc,
            FirstPath = new List<CommonClasses.IntPos>
            {
                new CommonClasses.IntPos{X = 403, Y = 1534},
                new CommonClasses.IntPos{X = 464, Y = 1526},
                new CommonClasses.IntPos{X = 558, Y = 1543},
                new CommonClasses.IntPos{X = 575, Y = 1534},
                new CommonClasses.IntPos{X = 677, Y = 1563},
                new CommonClasses.IntPos{X = 720, Y = 1584},
                new CommonClasses.IntPos{X = 771, Y = 1616},
                new CommonClasses.IntPos{X = 819, Y = 1652},
                new CommonClasses.IntPos{X = 861, Y = 1684},
                new CommonClasses.IntPos{X = 889, Y = 1720},
                new CommonClasses.IntPos{X = 877, Y = 1759},
                new CommonClasses.IntPos{X = 838, Y = 1785},
                new CommonClasses.IntPos{X = 877, Y = 1759},
                new CommonClasses.IntPos{X = 802, Y = 1815},
                new CommonClasses.IntPos{X = 806, Y = 1859},
            },
            RecieptName = "Quest4_3",
            Name = "Test of the blacksmith",
            Lvl = 8
            },
            new TaskCraftQuestReciept
            {
                Position = new CommonClasses.IntPos {X = 828, Y = 1900},
                Type = TaskTypesE.Craft,
                FirstPath = new List<CommonClasses.IntPos>
                {
                  
                },
                RecieptName = "Quest5_3",
                Name = "The path of autonomy",
                Lvl = 9
            },
            new Task
            {
                Position = new CommonClasses.IntPos {X = 144, Y = 2128},
                Type = TaskTypesE.KillNpc,
                FirstPath = new List<CommonClasses.IntPos>
                {
                },
                Name = "The share of the miner",
                Lvl = 10
            },
            new Task
            {
                Position = new CommonClasses.IntPos {X = 828, Y = 2242},
                Type = TaskTypesE.Harvest,
                FirstPath = new List<CommonClasses.IntPos>
                {
                },
                Name = "The fate of a stranger",
                Lvl = 11
            },  new Task
            {
                Position = new CommonClasses.IntPos {X = 426, Y = 2431},
                Type = TaskTypesE.AchiveItem,
                FirstPath = new List<CommonClasses.IntPos>
                {
                },
                Name = "The Last Citadel",
                Lvl = 12
            },  new Task
            {
                Position = new CommonClasses.IntPos {X = 941, Y = 2510},
                Type = TaskTypesE.Craft,
                FirstPath = new List<CommonClasses.IntPos>
                {
                },
                Name = "The path of autonomy",
                Lvl = 13
            },  new Task
            {
                Position = new CommonClasses.IntPos {X = 1125, Y = 2935},
                Type = TaskTypesE.Craft,
                FirstPath = new List<CommonClasses.IntPos>
                {
                },
                Name = "The path of autonomy",
                Lvl = 14
            }, new Task
            {
                Position = new CommonClasses.IntPos {X = 371, Y = 2777},
                Type = TaskTypesE.Craft,
                FirstPath = new List<CommonClasses.IntPos>
                {
                },
                Name = "The path of autonomy",
                Lvl = 15
            }, new Task
            {
                Position = new CommonClasses.IntPos {X = 715, Y = 3137},
                Type = TaskTypesE.Craft,
                FirstPath = new List<CommonClasses.IntPos>
                {
                },
                Name = "The path of autonomy",
                Lvl = 16
            },

        };


        public static int GetQuestLvl(string qname)
        {
            for (var a = 0; a < QuestLevel.Count; a++)
            {
                foreach (var b in QuestLevel[a])
                {
                    if (b == qname) return a;
                }
            }
            return 0;
        }

        public static void CraftLvlUp(Player player, Character cha)
        {
          //  if()
        }
        public static string GetNextQuest(string qname)
        {
            for (var a = 0; a < QuestLevel.Count; a++)
            {
                for(var b =0; b < QuestLevel[a].Length; b++)
                {
                    if (QuestLevel[a][b] != qname) continue;
                    if(b + 1 < QuestLevel[a].Length)
                    {
                        return QuestLevel[a][b + 1];
                    }
                    return QuestLevel[a][b];
                }
            }
            return "Quest0_1";
        }
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

        protected byte MasterNodeId ;

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
            DatabaseService.BsonMappingConfigurator.Configure();
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
            //if (this.IsResolver && MasterServerSettings.Default.EnableProxyConnections)
            //{
            //    // setup for proxy connections
            //    this.reader.NodeAdded += this.NodesReader_OnNodeAdded;
            //    this.reader.NodeChanged += this.NodesReader_OnNodeChanged;
            //    this.reader.NodeRemoved += this.NodesReader_OnNodeRemoved;
            //    log.Info("Proxy connections enabled");
            //}

            _reader.Start();

            // use local host id if nodes.txt does not exist or if line ending with 'Y' does not exist, otherwise use fixed node #1
            MasterNodeId = (byte) (LocalNodeId == 0 ? 0 : 1);

            Log.InfoFormat(
                "Current Node (nodeId={0}) is {1}the active master (leader)",
                _reader.CurrentNodeId,
                MasterNodeId == _reader.CurrentNodeId ? string.Empty : "NOT ");
        }

        #endregion
    }
}