using System.Collections.Generic;
using ExitGames.Logging;
using YourGame.Server.LoadBalancer;
using YourGame.Server.MasterServer.GameServer;
using YourGame.Server.MasterServer.Lobby;
using YourGame.Server.ServerToServer.Events;

namespace YourGame.Server.MasterServer
{
    public class GameApplication
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, GameState> _gameDict = new Dictionary<string, GameState>();
        public readonly string ApplicationId;
        public readonly LoadBalancer<IncomingGameServerPeer> LoadBalancer;
        public readonly PlayerCache PlayerOnlineCache;

        public GameApplication(string applicationId, LoadBalancer<IncomingGameServerPeer> loadBalancer)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Creating application: appId={0}", applicationId);
            }

            ApplicationId = applicationId;
            LoadBalancer = loadBalancer;
            PlayerOnlineCache = new PlayerCache();
            LobbyFactory = new LobbyFactory(this);
        }

        public LobbyFactory LobbyFactory { get; protected set; }

        public virtual void OnClientConnected(MasterClientPeer peer)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("OnClientConnect: peerId={0}, appId={1}", peer.ConnectionId, ApplicationId);
            }

            // remove from player cache
            if (PlayerOnlineCache != null && string.IsNullOrEmpty(peer.UserId) == false)
            {
                PlayerOnlineCache.OnConnectedToMaster(peer.UserId);
            }
        }

        public virtual void OnClientDisconnected(MasterClientPeer peer)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("OnClientDisconnect: peerId={0}, appId={1}", peer.ConnectionId, ApplicationId);
            }

            // remove from player cache
            if (PlayerOnlineCache != null && string.IsNullOrEmpty(peer.UserId) == false)
            {
                PlayerOnlineCache.OnDisconnectFromMaster(peer.UserId);
            }
        }

        public bool GetOrCreateGame(string gameId, AppLobby lobby, IGameList gameList, byte maxPlayer,
            IncomingGameServerPeer gameServerPeer, out GameState gameState)
        {
            lock (_gameDict)
            {
                if (_gameDict.TryGetValue(gameId, out gameState))
                {
                    return false;
                }

                gameState = new GameState(lobby, gameId, maxPlayer, gameServerPeer);
                _gameDict.Add(gameId, gameState);
                return true;
            }
        }

        public bool TryCreateGame(string gameId, AppLobby lobby, IGameList gameList, byte maxPlayer,
            IncomingGameServerPeer gameServerPeer, out GameState gameState)
        {
            var result = false;

            lock (_gameDict)
            {
                if (_gameDict.TryGetValue(gameId, out gameState) == false)
                {
                    gameState = new GameState(lobby, gameId, maxPlayer, gameServerPeer);
                    _gameDict.Add(gameId, gameState);
                    result = true;
                }
            }

            if (result)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Created game: gameId={0}, appId={1}", gameId, ApplicationId);
                }
            }

            return result;
        }

        public bool TryGetGame(string gameId, out GameState gameState)
        {
            lock (_gameDict)
            {
                return _gameDict.TryGetValue(gameId, out gameState);
            }
        }

        public void OnGameUpdateOnGameServer(UpdateGameEvent updateGameEvent, IncomingGameServerPeer gameServerPeer)
        {
            GameState gameState;

            lock (_gameDict)
            {
                if (!_gameDict.TryGetValue(updateGameEvent.GameId, out gameState))
                {
                    if (updateGameEvent.Reinitialize)
                    {
                        AppLobby lobby;
                        if (
                            !LobbyFactory.GetOrCreateAppLobby(updateGameEvent.LobbyId,
                                (AppLobbyType) updateGameEvent.LobbyType, out lobby))
                        {
                            if (Log.IsDebugEnabled)
                            {
                                Log.DebugFormat("Could not create lobby: name={0}, type={1}", updateGameEvent.LobbyId,
                                    (AppLobbyType) updateGameEvent.LobbyType);
                                return;
                            }
                        }

                        gameState = new GameState(lobby, updateGameEvent.GameId,
                            updateGameEvent.MaxPlayers.GetValueOrDefault(0), gameServerPeer);
                        _gameDict.Add(updateGameEvent.GameId, gameState);
                    }
                }
            }

            if (gameState != null)
            {
                if (gameState.GameServer != gameServerPeer)
                {
                    return;
                }

                gameState.Lobby.UpdateGameState(updateGameEvent, gameServerPeer);
                return;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Game to update not found: {0}", updateGameEvent.GameId);
            }
        }

        public void OnGameRemovedOnGameServer(string gameId)
        {
            bool found;
            GameState gameState;

            lock (_gameDict)
            {
                found = _gameDict.TryGetValue(gameId, out gameState);
            }

            if (found)
            {
                gameState.Lobby.RemoveGame(gameId);
            }
            else if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Game to remove not found: gameid={0}, appId={1}", gameId, ApplicationId);
            }
        }

        public bool RemoveGame(string gameId)
        {
            bool removed;

            lock (_gameDict)
            {
                removed = _gameDict.Remove(gameId);
            }

            if (Log.IsDebugEnabled)
            {
                if (removed)
                {
                    Log.DebugFormat("Removed game: gameId={0}, appId={1}", gameId, ApplicationId);
                }
                else
                {
                    Log.DebugFormat("Game to remove not found: gameId={0}, appId={1}", gameId, ApplicationId);
                }
            }

            return removed;
        }

        public virtual void OnGameServerRemoved(IncomingGameServerPeer gameServer)
        {
            LobbyFactory.OnGameServerRemoved(gameServer);
        }
    }
}