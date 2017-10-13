using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Server.Common;
using Warsmiths.Server.Events;
using Warsmiths.Server.MasterServer.Lobby;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.MasterServer.ChannelLobby
{
    public class GameChannel
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly HashSet<string> _changedGames = new HashSet<string>();
        private readonly GameChannelList _gameChannelList;

        private readonly LinkedListDictionary<string, GameState> _gameDict =
            new LinkedListDictionary<string, GameState>();

        private readonly GameChannelKey _key;
        private readonly string _propertyString;
        private readonly HashSet<string> _removedGames = new HashSet<string>();
        private readonly Dictionary<int, HashSet<PeerBase>> _subscriptions = new Dictionary<int, HashSet<PeerBase>>();

        public GameChannel(GameChannelList gameChannelList, GameChannelKey gamePropertyFilter)
        {
            _key = gamePropertyFilter;
            _gameChannelList = gameChannelList;

            foreach (var gameState in gameChannelList.GameDict.Values)
            {
                if (gameState.IsVisbleInLobby && GameProperties.IsSubsetOf(gameState.Properties))
                {
                    _gameDict.Add(gameState.Id, gameState);
                }
            }

            var sb = new StringBuilder();
            var seperator = false;
            foreach (DictionaryEntry entry in GameProperties)
            {
                if (seperator)
                {
                    sb.Append(" | ");
                }
                else
                {
                    seperator = true;
                }

                sb.AppendFormat("{0}:{1}", entry.Key, entry.Value);
            }

            _propertyString = sb.ToString();

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Created new game channel: {0}", _propertyString);
            }
        }

        public Hashtable GameProperties
        {
            get { return _key.Properties; }
        }

        public Subscription AddSubscription(PeerBase peer, int gameCount)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("New Subscription: pid={0}, gc={1}, props={2}", peer.ConnectionId, gameCount,
                    _propertyString);
            }

            if (gameCount < 0)
            {
                gameCount = 0;
            }

            var subscription = new Subscription(this, peer, gameCount);
            HashSet<PeerBase> hashSet;
            if (_subscriptions.TryGetValue(gameCount, out hashSet) == false)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Creating new hashset for game count = {0}", gameCount);
                }

                hashSet = new HashSet<PeerBase>();
                _subscriptions.Add(gameCount, hashSet);
            }

            hashSet.Add(peer);
            return subscription;
        }

        public void OnGameUpdated(GameState gameState)
        {
            if (GameProperties.IsSubsetOf(gameState.Properties))
            {
                if (_gameDict.ContainsKey(gameState.Id) == false)
                {
                    _gameDict.Add(gameState.Id, gameState);

                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Added game: gameId={0}, props={1}", gameState.Id, _propertyString);
                    }
                }

                _changedGames.Add(gameState.Id);
                _removedGames.Remove(gameState.Id);
            }
            else
            {
                if (_gameDict.ContainsKey(gameState.Id))
                {
                    _removedGames.Add(gameState.Id);
                    _changedGames.Remove(gameState.Id);

                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Removed game: gameId={0}, props={1}", gameState.Id, _propertyString);
                    }
                }
            }
        }

        public void OnGameRemoved(GameState gameState)
        {
            if (_gameDict.ContainsKey(gameState.Id))
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Removed game: gameId={0}, props={1}", gameState.Id, _propertyString);
                }

                _removedGames.Add(gameState.Id);
                _changedGames.Remove(gameState.Id);
            }
        }

        public void PublishGameChanges()
        {
            if (_removedGames.Count == 0 && _changedGames.Count == 0)
            {
                return;
            }

            foreach (var entry in _subscriptions)
            {
                Hashtable games;
                if (entry.Key == 0)
                {
                    games = GetChangedGames();
                }
                else
                {
                    games = GetChangedGames(entry.Key);
                }

                if (games.Count > 0)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Publishing game changes: props={0}", _propertyString);
                    }

                    var e = new GameListUpdateEvent {Data = games};
                    var eventData = new EventData((byte) EventCode.GameListUpdate, e);
                    eventData.SendTo(entry.Value, new SendParameters());
                }
            }

            foreach (var gameId in _removedGames)
            {
                _gameDict.Remove(gameId);
            }

            _changedGames.Clear();
            _removedGames.Clear();
        }

        public Hashtable GetChangedGames()
        {
            var result = new Hashtable(_removedGames.Count + _changedGames.Count);

            foreach (var gameId in _removedGames)
            {
                result.Add(gameId, new Hashtable {{(byte) GameParameter.Removed, true}});
            }

            foreach (var gameid in _changedGames)
            {
                GameState gameState;
                if (_gameDict.TryGet(gameid, out gameState))
                {
                    result.Add(gameState.Id, gameState.ToHashTable());
                }
            }

            return result;
        }

        public Hashtable GetChangedGames(int maxGameCount)
        {
            var result = new Hashtable();

            var i = 0;
            var removedCount = 0;

            var node = _gameDict.First;

            while (node != null && i < maxGameCount)
            {
                var gameState = node.Value;
                if (_removedGames.Contains(gameState.Id))
                {
                    result.Add(gameState.Id, new Hashtable {{(byte) GameParameter.Removed, true}});
                    removedCount++;
                }
                else if (_changedGames.Contains(gameState.Id))
                {
                    result.Add(gameState.Id, gameState.ToHashTable());
                }

                i++;
                node = node.Next;
            }

            i = 0;
            while (node != null & i < removedCount)
            {
                var gameState = node.Value;

                if (_removedGames.Contains(gameState.Id) == false)
                {
                    result.Add(gameState.Id, gameState.ToHashTable());
                    i++;
                }

                node = node.Next;
            }

            return result;
        }

        public Hashtable GetGameList(int maxCount)
        {
            if (maxCount <= 0)
            {
                maxCount = _gameDict.Count;
            }

            var hashTable = new Hashtable(maxCount);

            var i = 0;
            foreach (var game in _gameDict)
            {
                if (game.IsVisbleInLobby)
                {
                    var gameProperties = game.ToHashTable();
                    hashTable.Add(game.Id, gameProperties);
                }

                i++;

                if (i == maxCount)
                {
                    break;
                }
            }

            return hashTable;
        }

        public class Subscription : IGameListSubscibtion
        {
            public readonly GameChannel GameChannel;
            public readonly int GameCount;
            public readonly PeerBase Peer;
            private bool disposed;

            public Subscription(GameChannel channel, PeerBase peer, int count)
            {
                Peer = peer;
                GameChannel = channel;
                GameCount = count;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Disposing gamechannel subscription: pid={0}", Peer.ConnectionId);
                }

                HashSet<PeerBase> hashSet;
                if (GameChannel._subscriptions.TryGetValue(GameCount, out hashSet) == false)
                {
                    Log.WarnFormat("Failed to find game channel hashset for game count = {0}", GameCount);
                    return;
                }

                if (hashSet.Remove(Peer) == false)
                {
                    Log.WarnFormat("Failed to remove peer from channel: pid = {0}", Peer.ConnectionId);
                }

                if (hashSet.Count == 0)
                {
                    GameChannel._subscriptions.Remove(GameCount);
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Removed hashset for game count = {0}", GameCount);
                    }

                    if (GameChannel._subscriptions.Count == 0)
                    {
                        GameChannel._gameChannelList.GameChannels.Remove(GameChannel._key);
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Removed game channel: {0}", GameChannel._propertyString);
                        }
                    }
                }
            }

            public Hashtable GetGameList()
            {
                return GameChannel.GetGameList(GameCount);
            }
        }
    }
}