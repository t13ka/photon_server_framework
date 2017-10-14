using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Logging;
using YourGame.Server.LoadBalancer.Configuration;
using YourGame.Server.LoadShedding;

namespace YourGame.Server.LoadBalancer
{
    /// <summary>
    ///     Represents a collection of server instances which can be accessed
    ///     randomly based on their current lod level.
    /// </summary>
    /// <typeparam name="TServer">
    ///     The type of the server instances.
    /// </typeparam>
    /// <remarks>
    ///     Each server instance gets a weight assigned based on the current load level of that server.
    ///     The TryGetServer method gets a random server based on this weight. A server with a higher
    ///     weight will be returned more often than a server with a lower weight.
    ///     The default values for this weights are the following:
    ///     LoadLevel.Lowest  = 40
    ///     LoadLevel.Low     = 30
    ///     LoadLevel.Normal  = 20
    ///     LoadLevel.High    = 10
    ///     LoadLevel.Highest = 0
    ///     If there is for example one server for eac load level, the server with load level lowest
    ///     will be returned 50% of the times, the one with load level low 30% and so on.
    /// </remarks>
    public class LoadBalancer<TServer>
    {
        #region Properties

        /// <summary>
        ///     Gets the average workload of all server instances.
        /// </summary>
        public FeedbackLevel AverageWorkload
        {
            get
            {
                if (_serverList.Count == 0)
                {
                    return 0;
                }

                return (FeedbackLevel)(int)Math.Round((double)_totalWorkload / _serverList.Count);
            }
        }

        #endregion

        private void InitializeFromConfig(string configFilePath)
        {
            string message;
            LoadBalancerSection section;

            int[] weights = null;

            if (!ConfigurationLoader.TryLoadFromFile(configFilePath, out section, out message))
            {
                Log.WarnFormat(
                    "Could not initialize LoadBalancer from configuration: Invalid configuration file {0}. Using default settings... ({1})",
                    configFilePath,
                    message);
            }

            if (section != null)
            {
                // load weights from config file & sort:
                var dict = new SortedDictionary<int, int>();
                foreach (LoadBalancerWeight weight in section.LoadBalancerWeights)
                {
                    dict.Add((int)weight.Level, weight.Value);
                }

                if (dict.Count == (int)FeedbackLevel.Highest + 1)
                {
                    weights = new int[dict.Count];
                    dict.Values.CopyTo(weights, 0);

                    Log.InfoFormat("Initialized Load Balancer from configuration file: {0}", configFilePath);
                }
                else
                {
                    Log.WarnFormat(
                        "Could not initialize LoadBalancer from configuration: {0} is invalid - expected {1} entries, but found {2}. Using default settings...",
                        configFilePath,
                        (int)FeedbackLevel.Highest + 1,
                        dict.Count);
                }
            }

            if (weights == null)
            {
                weights = DefaultConfiguration.GetDefaultWeights();
            }

            _loadLevelWeights = weights;
        }

        private void UpdateTotalWorkload(FeedbackLevel oldLoadLevel, FeedbackLevel newLoadLevel)
        {
            _totalWorkload -= (int)oldLoadLevel;
            _totalWorkload += (int)newLoadLevel;
        }

        private int GetLoadLevelWeight(FeedbackLevel loadLevel)
        {
            return _loadLevelWeights[(int)loadLevel];
        }

        private void AddToAvailableServers(ServerState serverState)
        {
            _totalWeight += serverState.Weight;

            // find the first server with a lower weight and insert
            // the server before it to keep the list of available server 
            // instances sorted by weight.
            var node = _availableServers.First;
            while (node != null)
            {
                if (node.Value.Weight <= serverState.Weight)
                {
                    serverState.Node = _availableServers.AddBefore(node, serverState);
                    return;
                }

                node = node.Next;
            }

            // no server with a lower load level has been found
            // so simply add the server to the end of the available 
            // server list
            serverState.Node = _availableServers.AddLast(serverState);
        }

        private void RemoveFromAvailableServers(ServerState serverState)
        {
            if (serverState.Node == null)
            {
                return;
            }

            _totalWeight -= serverState.Weight;
            _availableServers.Remove(serverState.Node);
            serverState.Node = null;
        }

        private void ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            Log.InfoFormat("Configuration file for LoadBalancer Weights {0}\\{1} {2}. Reinitializing...", e.FullPath,
                e.Name, e.ChangeType);

            InitializeFromConfig(e.FullPath);

            UpdateWeightForAllServers();
        }

        private void UpdateWeightForAllServers()
        {
            lock (_serverList)
            {
                foreach (var server in _serverList.Keys)
                {
                    // check if server instance exits
                    ServerState serverState;
                    if (_serverList.TryGetValue(server, out serverState) == false)
                    {
                        continue;
                    }

                    var newWeight = GetLoadLevelWeight(serverState.LoadLevel);

                    // check if the weight for the server instance has changes
                    // if it has not changed we don't have to update the list of available servers
                    if (newWeight == serverState.Weight)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat(
                                "LoadBalancer Weight did NOT change for server {0}: loadLevel={1}, weight={2}",
                                serverState.Server, serverState.LoadLevel, serverState.Weight);
                        }
                    }
                    else
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat(
                                "LoadBalancer Weight did change for server {0}: loadLevel={1}, oldWeight={2}, newWeight={3}",
                                serverState.Server,
                                serverState.LoadLevel,
                                serverState.Weight,
                                newWeight);
                        }

                        RemoveFromAvailableServers(serverState);
                        serverState.Weight = newWeight;

                        if (serverState.Weight > 0)
                        {
                            AddToAvailableServers(serverState);
                        }
                    }
                }
            }
        }

        private class ServerState
        {
            public ServerState(TServer server)
            {
                Server = server;
            }

            public TServer Server { get; }
            public FeedbackLevel LoadLevel ;
            public int Weight ;
            public LinkedListNode<ServerState> Node ;
        }

        #region Constants and Fields

        // ReSharper disable StaticFieldInGenericType
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        // ReSharper restore StaticFieldInGenericType

        // dictionary for fast server instance lookup
        private readonly Dictionary<TServer, ServerState> _serverList;

        // stores the available server instances ordered by their weight
        private readonly LinkedList<ServerState> _availableServers = new LinkedList<ServerState>();

        // list of the weights for each possible load level
        private int[] _loadLevelWeights;

        // pseudo-random number generator for gettings a random server
        private readonly Random _random;

        // stores the sum of the weights of all server instances
        private int _totalWeight;

        // stores the sum of the load levels of all server instances
        // used to calculate the average load level
        private int _totalWorkload;

        // watch files 
        private readonly FileSystemWatcher _fileWatcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadBalancer{TServer}" /> class. Use default weights for each load
        ///     level.
        /// </summary>
        public LoadBalancer()
        {
            _random = new Random();
            _serverList = new Dictionary<TServer, ServerState>();
            _loadLevelWeights = DefaultConfiguration.GetDefaultWeights();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadBalancer{TServer}" /> class.
        /// </summary>
        /// <param name="configFilePath">
        ///     The full path (absolute or relative) to a config file that specifies a Weight for each LoadLevel.
        ///     The possible load levels and their values are defined  int the
        ///     <see cref="FeedbackLevel" /> enumeration.
        ///     See the LoadBalancing.config for an example.
        /// </param>
        public LoadBalancer(string configFilePath)
        {
            _serverList = new Dictionary<TServer, ServerState>();
            _random = new Random();

            InitializeFromConfig(configFilePath);

            var fullPath = Path.GetFullPath(configFilePath);
            var path = Path.GetDirectoryName(fullPath);
            if (path == null)
            {
                Log.InfoFormat("Could not watch for configuration file. No path specified.");
                return;
            }

            var filter = Path.GetFileName(fullPath);
            if (filter == null)
            {
                Log.InfoFormat("Could not watch for configuration file. No file specified.");
                return;
            }

            _fileWatcher = new FileSystemWatcher(path, filter);
            _fileWatcher.Changed += ConfigFileChanged;
            _fileWatcher.Created += ConfigFileChanged;
            _fileWatcher.Deleted += ConfigFileChanged;
            _fileWatcher.Renamed += ConfigFileChanged;
            _fileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadBalancer{TServer}" /> class.
        /// </summary>
        /// <param name="loadLevelWeights">
        ///     A list of weights which should be used for each available load level.
        ///     This list must contain a value for each available load level and
        ///     must be ordered by the load levels value.
        ///     The possible load levels and their values are defined  int the
        ///     <see cref="FeedbackLevel" /> enumeration.
        /// </param>
        public LoadBalancer(int[] loadLevelWeights)
        {
            if (loadLevelWeights == null)
            {
                throw new ArgumentNullException("loadLevelWeights");
            }

            const int feedbackLevelCount = (int)FeedbackLevel.Highest + 1;
            if (loadLevelWeights.Length != feedbackLevelCount)
            {
                throw new ArgumentOutOfRangeException(
                    "loadLevelWeights",
                    string.Format(
                        "Parameter loadLevelWeights must have a length of {0}. One weight for each possible load level",
                        feedbackLevelCount));
            }

            _serverList = new Dictionary<TServer, ServerState>();
            _random = new Random();
            this._loadLevelWeights = loadLevelWeights;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadBalancer{TServer}" /> class.
        /// </summary>
        /// <remarks>
        ///     This overload is used for unit testing to provide a fixed seed for the
        ///     random number generator.
        /// </remarks>
        public LoadBalancer(int[] loadLevelWeights, int seed)
            : this(loadLevelWeights)
        {
            _random = new Random(seed);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Attempts to add a server instance.
        /// </summary>
        /// <param name="server">The server instance to add.</param>
        /// <param name="loadLevel">The current workload of the server instance.</param>
        /// <returns>
        ///     True if the server instance was added successfully. If the server instance already exists,
        ///     this method returns false.
        /// </returns>
        public bool TryAddServer(TServer server, FeedbackLevel loadLevel)
        {
            lock (_serverList)
            {
                // check if the server instance was already added
                if (_serverList.ContainsKey(server))
                {
                    return false;
                }

                var serverState = new ServerState(server)
                {
                    LoadLevel = loadLevel,
                    Weight = GetLoadLevelWeight(loadLevel)
                };

                _serverList.Add(server, serverState);

                if (serverState.Weight > 0)
                {
                    AddToAvailableServers(serverState);
                }

                UpdateTotalWorkload(FeedbackLevel.Lowest, loadLevel);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Added server: workload={0}", loadLevel);
                }

                return true;
            }
        }

        /// <summary>
        ///     Tries to get a free server instance.
        /// </summary>
        /// <param name="server">
        ///     When this method returns, contains an available server instance
        ///     or null if no available server instances exists.
        /// </param>
        /// <returns>
        ///     True if a server instance with enough remaining workload is found; otherwise false.
        /// </returns>
        public bool TryGetServer(out TServer server)
        {
            FeedbackLevel workload;
            return TryGetServer(out server, out workload);
        }

        /// <summary>
        ///     Tries to get a free server instance.
        /// </summary>
        /// <param name="server">
        ///     When this method returns, contains the server instance with the fewest workload
        ///     or null if no server instances exists.
        /// </param>
        /// <param name="loadLevel">
        ///     When this method returns, contains an available server instance
        ///     or null if no available server instances exists.
        /// </param>
        /// <returns>
        ///     True if a server instance with enough remaining workload is found; otherwise false.
        /// </returns>
        public bool TryGetServer(out TServer server, out FeedbackLevel loadLevel)
        {
            lock (_serverList)
            {
                if (_availableServers.Count == 0)
                {
                    loadLevel = FeedbackLevel.Highest;
                    server = default(TServer);
                    return false;
                }

                // Get a random weight between 0 and the sum of the weight of all server isntances
                var randomWeight = _random.Next(_totalWeight);
                var weight = 0;

                // Iterate through the server instances and add sum the weights of each instance.
                // If the sum of the weights is greater than the generated random value
                // the current server instance in the loop will be returned.
                // Using this method ensures that server instances with a higher weight will
                // be hit more often than one with a lower weihgt.
                var node = _availableServers.First;
                while (node != null)
                {
                    weight += node.Value.Weight;
                    if (weight > randomWeight)
                    {
                        server = node.Value.Server;
                        loadLevel = node.Value.LoadLevel;
                        return true;
                    }

                    node = node.Next;
                }

                // this should never happen but better log out a warning and 
                // return an available server instance
                Log.WarnFormat("Failed to get a server instance based on the weights");
                server = _availableServers.First.Value.Server;
                loadLevel = _availableServers.First.Value.LoadLevel;
                return true;
            }
        }

        /// <summary>
        ///     Tries to remove a server instance.
        /// </summary>
        /// <param name="server">The server instance to remove.</param>
        /// <returns>
        ///     True if the server instance was removed successfully.
        ///     If the server instance does not exists, this method returns false.
        /// </returns>
        public bool TryRemoveServer(TServer server)
        {
            lock (_serverList)
            {
                ServerState serverState;
                if (_serverList.TryGetValue(server, out serverState) == false)
                {
                    return false;
                }

                _serverList.Remove(server);
                RemoveFromAvailableServers(serverState);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Removed server: workload={0}", serverState.LoadLevel);
                }

                return true;
            }
        }

        /// <summary>
        ///     Tries to update a server instance.
        /// </summary>
        /// <param name="server">The server to update.</param>
        /// <param name="newLoadLevel">The current workload of the server instance.</param>
        /// <returns>
        ///     True if the server instance was updated successfully.
        ///     If the server instance does not exists, this method returns false.
        /// </returns>
        public bool TryUpdateServer(TServer server, FeedbackLevel newLoadLevel)
        {
            lock (_serverList)
            {
                // check if server instance exits
                ServerState serverState;
                if (_serverList.TryGetValue(server, out serverState) == false)
                {
                    return false;
                }

                // check if load level has changed
                if (serverState.LoadLevel == newLoadLevel)
                {
                    return true;
                }

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Updating server: oldWorkload={0}, newWorkload={1}", serverState.LoadLevel,
                        newLoadLevel);
                }

                // apply new state
                UpdateTotalWorkload(serverState.LoadLevel, newLoadLevel);

                serverState.LoadLevel = newLoadLevel;
                var newWeight = GetLoadLevelWeight(newLoadLevel);

                // check if the weight for the server instance has changes
                // if it has not changed we don't have to update the list of available servers
                if (newWeight == serverState.Weight)
                {
                    return true;
                }

                RemoveFromAvailableServers(serverState);
                serverState.Weight = newWeight;

                if (serverState.Weight > 0)
                {
                    AddToAvailableServers(serverState);
                }

                return true;
            }
        }

        #endregion
    }
}