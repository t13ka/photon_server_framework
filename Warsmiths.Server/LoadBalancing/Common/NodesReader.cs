using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ExitGames.Logging;

namespace Warsmiths.Server.Common
{
    public class NodesReader : IDisposable
    {
        #region Constructors and Destructors

        public NodesReader(string nodesFilePath, string nodesFileName)
        {
            CurrentNodeId = 0;
            _nodesFilePath = nodesFilePath;
            _nodesFileName = nodesFileName;

            if (!string.IsNullOrEmpty(nodesFilePath) && Directory.Exists(nodesFilePath))
            {
                _watcher = new FileSystemWatcher(_nodesFilePath, _nodesFileName);
            }
        }

        #endregion

        #region Properties

        public byte CurrentNodeId { get; private set; }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }
        }

        #endregion

        #endregion

        public class NodeEventArgs : EventArgs
        {
            #region Constructors and Destructors

            public NodeEventArgs(byte nodeId, IPAddress address)
            {
                Address = address;
                NodeId = nodeId;
            }

            #endregion

            #region Constants and Fields

            #endregion

            #region Properties

            public IPAddress Address { get; }

            public byte NodeId { get; }

            #endregion
        }

        #region Constants and Fields

        private readonly string _nodesFileName;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly string _nodesFilePath;
        private readonly object _syncRoot = new object();
        private readonly FileSystemWatcher _watcher;
        private Dictionary<byte, IPAddress> _nodes = new Dictionary<byte, IPAddress>();

        #endregion

        #region Events

        public event EventHandler<NodeEventArgs> NodeAdded;
        public event EventHandler<NodeEventArgs> NodeChanged;
        public event EventHandler<NodeEventArgs> NodeRemoved;

        #endregion

        #region Public Methods

        public IPAddress GetIpAddress(byte nodeId)
        {
            lock (_syncRoot)
            {
                IPAddress result;
                if (_nodes.TryGetValue(nodeId, out result) == false)
                {
                    Log.WarnFormat("Internal address for node {0} unknown; using loop back", nodeId);
                    result = IPAddress.Loopback;
                    _nodes.Add(nodeId, result);
                    Log.Info("Node added: " + nodeId + " = " + result);
                }

                return result;
            }
        }

        public void OnNodeAdded(NodeEventArgs e)
        {
            var handler = NodeAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnNodeChanged(NodeEventArgs e)
        {
            var handler = NodeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnNodeRemoved(NodeEventArgs e)
        {
            var handler = NodeRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public byte ReadCurrentNodeId()
        {
            var path = Path.Combine(_nodesFilePath, _nodesFileName);
            if (File.Exists(path))
            {
                ReadNodes(path);
            }

            return CurrentNodeId;
        }

        public void Start()
        {
            ImportNodes();

            if (_watcher != null)
            {
                Log.Info("Watching " + Path.Combine(_nodesFilePath, _nodesFileName));
                _watcher.Changed += FileSystemWatcher_OnChanged;
                _watcher.Created += FileSystemWatcher_OnCreated;
                _watcher.Deleted += FileSystemWatcher_OnDeleted;
                _watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
            }
        }

        #endregion

        #region Methods

        private void ImportNodes()
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info("Import nodes");
            }

            var path = Path.Combine(_nodesFilePath, _nodesFileName);
            if (File.Exists(path))
            {
                var inputNodes = ReadNodes(path);
                UpdateNodes(inputNodes);
            }
            else
            {
                CurrentNodeId = 0;
                Log.Warn(path + " does not exist, CurrentNodeId = " + CurrentNodeId);
            }
        }

        private void FileSystemWatcher_OnChanged(object sender, FileSystemEventArgs e)
        {
            var newEntries = ReadNodes(e.FullPath);
            UpdateNodes(newEntries);
        }

        private void FileSystemWatcher_OnCreated(object sender, FileSystemEventArgs e)
        {
            var newEntries = ReadNodes(e.FullPath);
            UpdateNodes(newEntries);
        }

        private void FileSystemWatcher_OnDeleted(object sender, FileSystemEventArgs e)
        {
            UpdateNodes(new Dictionary<byte, IPAddress>());
        }

        private Dictionary<byte, IPAddress> ReadNodes(string path)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info("Reading nodes");
            }

            var newEntries = new Dictionary<byte, IPAddress>();
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat("Node line read: {0}", line);
                    }

                    var split = line.Split(' ');
                    if (split.Length < 3)
                    {
                        Log.Warn("Skipped invalid line format (expected [id] [IP] [Y/N])");
                        continue;
                    }

                    byte nodeId;
                    if (!byte.TryParse(split[0], out nodeId))
                    {
                        continue;
                    }

                    if (newEntries.ContainsKey(nodeId))
                    {
                        // duplicate entry
                        Log.Warn("Skipped duplicate node id");
                        continue;
                    }

                    IPAddress address;
                    if (!IPAddress.TryParse(split[1], out address))
                    {
                        Log.Warn("Skipped invalid line (wrong IP format)");
                        continue;
                    }

                    if (split[2] == "Y")
                    {
                        CurrentNodeId = nodeId;
                        Log.Info("Local nodeId is " + CurrentNodeId);
                    }

                    newEntries.Add(nodeId, address);
                }
            }

            Log.InfoFormat("Read {0} nodes from {1}", newEntries.Count, _nodesFileName);
            return newEntries;
        }

        private void UpdateNodes(Dictionary<byte, IPAddress> newEntries)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info("Updating Nodes...");
            }

            lock (_syncRoot)
            {
                foreach (var entry in _nodes)
                {
                    IPAddress newAddress;
                    if (newEntries.TryGetValue(entry.Key, out newAddress))
                    {
                        // operator == returns false even though Equals returns true
                        if (newAddress.Equals(entry.Value) == false)
                        {
                            Log.Info("Node changed: " + entry.Key + " = " + newAddress);
                            OnNodeChanged(new NodeEventArgs(entry.Key, newAddress));
                        }
                    }
                    else
                    {
                        // node removed
                        Log.Info("Node removed: " + entry.Key + " = " + entry.Value);
                        OnNodeRemoved(new NodeEventArgs(entry.Key, null));
                    }
                }

                foreach (var entry in newEntries)
                {
                    if (!_nodes.ContainsKey(entry.Key))
                    {
                        // node added
                        Log.Info("Node added: " + entry.Key + " = " + entry.Value);
                        OnNodeAdded(new NodeEventArgs(entry.Key, entry.Value));
                    }
                }

                _nodes = newEntries;
            }
        }

        #endregion
    }
}