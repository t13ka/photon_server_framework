using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ExitGames.Logging;
using Photon.SocketServer;

using YourGame.Server.Events;
using YourGame.Server.MasterServer.GameServer;
using YourGame.Server.Operations;
using YourGame.Server.ServerToServer.Events;

namespace YourGame.Server.MasterServer.Lobby
{
    using YourGame.Common;

    public class GameList : IGameList
    {
        #region Constructors and Destructors

        public GameList(AppLobby lobby)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Creating new GameList");
            }

            Lobby = lobby;
            _gameDict = new LinkedListDictionary<string, GameState>();
            _changedGames = new Dictionary<string, GameState>();
            _removedGames = new HashSet<string>();
        }

        #endregion

        private class Subscribtion : IGameListSubscibtion
        {
            private readonly int _maxGameCount;
            private readonly PeerBase _peer;
            private GameList _gameList;

            public Subscribtion(GameList gameList, PeerBase peer, int maxGameCount)
            {
                _gameList = gameList;
                _peer = peer;
                _maxGameCount = maxGameCount;
            }

            public void Dispose()
            {
                var gl = Interlocked.Exchange(ref _gameList, null);
                if (gl != null)
                {
                    gl._peers.Remove(_peer);
                }
            }

            public Hashtable GetGameList()
            {
                var gl = _gameList;
                if (gl == null)
                {
                    // subscription has been disposed (client has diconnect) during the request handling
                    return new Hashtable();
                }

                return gl.GetAllGames(_maxGameCount);
            }
        }

        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        public readonly AppLobby Lobby;
        private readonly Dictionary<string, GameState> _changedGames;
        private readonly LinkedListDictionary<string, GameState> _gameDict;
        private readonly HashSet<string> _removedGames;
        private readonly HashSet<PeerBase> _peers = new HashSet<PeerBase>();
        private readonly Random _rnd = new Random();
        private LinkedListNode<GameState> _nextJoinRandomStartNode;

        #endregion

        #region Properties

        public int ChangedGamesCount
        {
            get { return _changedGames.Count + _removedGames.Count; }
        }

        public int Count
        {
            get { return _gameDict.Count; }
        }

        #endregion

        #region Public Methods

        public void AddGameState(GameState gameState)
        {
            _gameDict.Add(gameState.Id, gameState);
        }

        public void CheckJoinTimeOuts(int timeOutSeconds)
        {
            CheckJoinTimeOuts(TimeSpan.FromSeconds(timeOutSeconds));
        }

        public int CheckJoinTimeOuts(TimeSpan timeOut)
        {
            var minDate = DateTime.UtcNow.Subtract(timeOut);
            return CheckJoinTimeOuts(minDate);
        }

        public int CheckJoinTimeOuts(DateTime minDateTime)
        {
            var oldJoiningCount = 0;
            var joiningPlayerCount = 0;

            var toRemove = new List<GameState>();

            foreach (var gameState in _gameDict)
            {
                if (gameState.JoiningPlayerCount > 0)
                {
                    oldJoiningCount += gameState.JoiningPlayerCount;
                    gameState.CheckJoinTimeOuts(minDateTime);

                    // check if there are still players left for the game
                    if (gameState.PlayerCount == 0)
                    {
                        toRemove.Add(gameState);
                    }

                    joiningPlayerCount += gameState.JoiningPlayerCount;
                }
            }

            // remove all games where no players left
            foreach (var gameState in toRemove)
            {
                RemoveGameState(gameState.Id);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Checked join timeouts: before={0}, after={1}", oldJoiningCount, joiningPlayerCount);
            }

            return joiningPlayerCount;
        }

        public bool ContainsGameId(string gameId)
        {
            return _gameDict.ContainsKey(gameId);
        }

        public Hashtable GetAllGames(int maxCount)
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
                    i++;
                }

                if (i == maxCount)
                {
                    break;
                }
            }

            return hashTable;
        }

        public Hashtable GetChangedGames()
        {
            if (_changedGames.Count == 0 && _removedGames.Count == 0)
            {
                return null;
            }

            var hashTable = new Hashtable(_changedGames.Count + _removedGames.Count);

            foreach (var gameInfo in _changedGames.Values)
            {
                if (gameInfo.IsVisbleInLobby)
                {
                    var gameProperties = gameInfo.ToHashTable();
                    hashTable.Add(gameInfo.Id, gameProperties);
                }
            }

            foreach (var gameId in _removedGames)
            {
                hashTable.Add(gameId, new Hashtable {{(byte) GameParameter.Removed, true}});
            }

            _changedGames.Clear();
            _removedGames.Clear();

            return hashTable;
        }

        public void RemoveGameServer(IncomingGameServerPeer gameServer)
        {
            // find games belonging to the game server instance
            var instanceStates = _gameDict.Where(gameState => gameState.GameServer == gameServer).ToList();

            // remove game server instance games
            foreach (var gameState in instanceStates)
            {
                RemoveGameState(gameState.Id);
            }
        }

        public bool RemoveGameState(string gameId)
        {
            Lobby.Application.RemoveGame(gameId);

            GameState gameState;
            if (!_gameDict.TryGet(gameId, out gameState))
            {
                return false;
            }

            if (Log.IsDebugEnabled)
            {
                LogGameState("RemoveGameState:", gameState);
            }

            if (_nextJoinRandomStartNode != null && _nextJoinRandomStartNode.Value == gameState)
            {
                AdvanceNextJoinRandomStartNode();
            }

            gameState.OnRemoved();

            _gameDict.Remove(gameId);
            _changedGames.Remove(gameId);
            _removedGames.Add(gameId);

            return true;
        }

        public bool TryGetGame(string gameId, out GameState gameState)
        {
            return _gameDict.TryGet(gameId, out gameState);
        }

        public ErrorCode TryGetRandomGame(JoinRandomType joinType, ILobbyPeer peer, Hashtable gameProperties,
            string query, out GameState gameState, out string message)
        {
            message = null;

            if (_gameDict.Count == 0)
            {
                gameState = null;
                return ErrorCode.NoMatchFound;
            }

            LinkedListNode<GameState> startNode;
            switch (joinType)
            {
                default:
                    startNode = _gameDict.First;
                    break;

                case JoinRandomType.FromLastMatch:
                    startNode = _nextJoinRandomStartNode ?? _gameDict.First;
                    break;

                case JoinRandomType.Random:
                    var startNodeIndex = _rnd.Next(_gameDict.Count);
                    startNode = _gameDict.GetAtIntext(startNodeIndex);
                    break;
            }

            if (!TryGetRandomGame(startNode, gameProperties, out gameState))
            {
                return ErrorCode.NoMatchFound;
            }

            return ErrorCode.Ok;
        }

        public bool UpdateGameState(UpdateGameEvent updateOperation, IncomingGameServerPeer incomingGameServerPeer,
            out GameState gameState)
        {
            // try to get the game state 
            if (_gameDict.TryGet(updateOperation.GameId, out gameState) == false)
            {
                if (updateOperation.Reinitialize)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Reinitialize: Add Game State {0}", updateOperation.GameId);
                    }

                    if (!Lobby.Application.TryGetGame(updateOperation.GameId, out gameState))
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Could not find game to reinitialize: {0}", updateOperation.GameId);
                        }

                        return false;
                    }

                    _gameDict.Add(updateOperation.GameId, gameState);
                }
                else
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Game not found: {0}", updateOperation.GameId);
                    }
                    return false;
                }
            }

            var oldVisible = gameState.IsVisbleInLobby;
            var changed = gameState.Update(updateOperation);

            if (changed)
            {
                if (gameState.IsVisbleInLobby)
                {
                    _changedGames[gameState.Id] = gameState;

                    if (oldVisible == false)
                    {
                        _removedGames.Remove(gameState.Id);
                    }
                }
                else
                {
                    if (oldVisible)
                    {
                        _changedGames.Remove(gameState.Id);
                        _removedGames.Add(gameState.Id);
                    }
                }

                if (Log.IsDebugEnabled)
                {
                    LogGameState("UpdateGameState: ", gameState);
                }
            }

            return true;
        }

        public IGameListSubscibtion AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount)
        {
            var subscribtion = new Subscribtion(this, peer, maxGameCount);
            _peers.Add(peer);
            return subscribtion;
        }

        public void PublishGameChanges()
        {
            if (ChangedGamesCount > 0)
            {
                var gameList = GetChangedGames();

                var e = new GameListUpdateEvent {Data = gameList};
                var eventData = new EventData((byte) EventCode.GameListUpdate, e);
                ApplicationBase.Instance.BroadCastEvent(eventData, _peers, new SendParameters());
            }
        }

        public void OnMaxPlayerReached(GameState gameState)
        {
        }

        public void OnPlayerCountFallBelowMaxPlayer(GameState gameState)
        {
        }

        #endregion

        #region Methods

        private static void LogGameState(string prefix, GameState gameState)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(
                    "{0}id={1}, peers={2}, max={3}, open={4}, visible={5}, peersJoining={6}",
                    prefix,
                    gameState.Id,
                    gameState.GameServerPlayerCount,
                    gameState.MaxPlayer,
                    gameState.IsOpen,
                    gameState.IsVisible,
                    gameState.JoiningPlayerCount);
            }
        }

        private void AdvanceNextJoinRandomStartNode()
        {
            if (_nextJoinRandomStartNode == null)
            {
                return;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(
                    "Changed last join random match: oldGameId={0}, newGameId={1}",
                    _nextJoinRandomStartNode.Value.Id,
                    _nextJoinRandomStartNode.Next == null ? "{null}" : _nextJoinRandomStartNode.Value.Id);
            }

            _nextJoinRandomStartNode = _nextJoinRandomStartNode.Next;
        }

        private bool TryGetRandomGame(LinkedListNode<GameState> startNode, Hashtable gameProperties,
            out GameState gameState)
        {
            var node = startNode;

            do
            {
                var game = node.Value;
                node = node.Next ?? _gameDict.First;

                if (!game.IsOpen || !game.IsVisible || !game.IsCreatedOnGameServer ||
                    (game.MaxPlayer > 0 && game.PlayerCount >= game.MaxPlayer))
                {
                    continue;
                }

                if (gameProperties != null && game.MatchGameProperties(gameProperties) == false)
                {
                    continue;
                }

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Found match. Next start node gameid={0}", node.Value.Id);
                }

                _nextJoinRandomStartNode = node;
                gameState = game;
                return true;
            } while (node != startNode);

            gameState = null;
            return false;
        }

        #endregion
    }
}