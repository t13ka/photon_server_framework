using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Logging;
using Photon.SocketServer;

using YourGame.Server.MasterServer.GameServer;
using YourGame.Server.MasterServer.Lobby;
using YourGame.Server.Operations;
using YourGame.Server.ServerToServer.Events;

namespace YourGame.Server.MasterServer.ChannelLobby
{
    using YourGame.Common;

    public class GameChannelList : IGameList
    {
        #region Constructors and Destructors

        public GameChannelList(AppLobby lobby)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Creating new GameChannelList");
            }

            Lobby = lobby;
            GameDict = new Dictionary<string, GameState>();
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return GameDict.Count; }
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

        #endregion

        #region Constants and Fields

        internal readonly Dictionary<string, GameState> GameDict;

        internal readonly Dictionary<GameChannelKey, GameChannel> GameChannels =
            new Dictionary<GameChannelKey, GameChannel>();

        public readonly AppLobby Lobby;

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods

        public void AddGameState(GameState gameState)
        {
            GameDict.Add(gameState.Id, gameState);
        }

        public int CheckJoinTimeOuts(int timeOutSeconds)
        {
            return CheckJoinTimeOuts(TimeSpan.FromSeconds(timeOutSeconds));
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

            foreach (var gameState in GameDict.Values)
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
            return GameDict.ContainsKey(gameId);
        }

        public IGameListSubscibtion AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount)
        {
            if (gamePropertyFilter == null)
            {
                gamePropertyFilter = new Hashtable(0);
            }

            GameChannel gameChannel;
            var key = new GameChannelKey(gamePropertyFilter);

            if (!GameChannels.TryGetValue(key, out gameChannel))
            {
                gameChannel = new GameChannel(this, key);
                GameChannels.Add(key, gameChannel);
            }

            return gameChannel.AddSubscription(peer, maxGameCount);
        }


        public void RemoveGameServer(IncomingGameServerPeer gameServer)
        {
            // find games belonging to the game server instance
            var instanceGames = new List<GameState>();
            foreach (var gameState in GameDict.Values)
            {
                if (gameState.GameServer == gameServer)
                {
                    instanceGames.Add(gameState);
                }
            }

            // remove game server instance games
            foreach (var gameState in instanceGames)
            {
                RemoveGameState(gameState.Id);
            }
        }

        public bool RemoveGameState(string gameId)
        {
            Lobby.Application.RemoveGame(gameId);

            GameState gameState;
            if (!GameDict.TryGetValue(gameId, out gameState))
            {
                return false;
            }

            if (Log.IsDebugEnabled)
            {
                LogGameState("RemoveGameState:", gameState);
            }

            foreach (var channel in GameChannels.Values)
            {
                channel.OnGameRemoved(gameState);
            }

            GameDict.Remove(gameId);
            return true;
        }

        public bool TryGetGame(string gameId, out GameState gameState)
        {
            return GameDict.TryGetValue(gameId, out gameState);
        }

        public ErrorCode TryGetRandomGame(JoinRandomType joinType, ILobbyPeer peer, Hashtable gameProperties,
            string query, out GameState gameState, out string message)
        {
            message = null;

            foreach (var game in GameDict.Values)
            {
                if (game.IsOpen && game.IsVisible && game.IsCreatedOnGameServer &&
                    (game.MaxPlayer <= 0 || game.PlayerCount < game.MaxPlayer))
                {
                    if (gameProperties != null && game.MatchGameProperties(gameProperties) == false)
                    {
                        continue;
                    }

                    gameState = game;
                    return ErrorCode.Ok;
                }
            }

            gameState = null;
            return ErrorCode.NoMatchFound;
        }

        public bool UpdateGameState(UpdateGameEvent updateOperation, IncomingGameServerPeer gameServerPeer,
            out GameState gameState)
        {
            // try to get the game state 
            if (GameDict.TryGetValue(updateOperation.GameId, out gameState) == false)
            {
                if (updateOperation.Reinitialize)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Reinitialize: Add Game State {0}", updateOperation.GameId);
                    }

                    gameState = new GameState(Lobby, updateOperation.GameId, 0, gameServerPeer);
                    GameDict.Add(updateOperation.GameId, gameState);
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

            if (!changed)
            {
                return false;
            }

            if (Log.IsDebugEnabled)
            {
                LogGameState("UpdateGameState: ", gameState);
            }

            if (gameState.IsVisbleInLobby)
            {
                foreach (var channel in GameChannels.Values)
                {
                    channel.OnGameUpdated(gameState);
                }

                return true;
            }

            if (oldVisible)
            {
                foreach (var channel in GameChannels.Values)
                {
                    channel.OnGameRemoved(gameState);
                }
            }

            return true;
        }

        public void PublishGameChanges()
        {
            foreach (var channel in GameChannels.Values)
            {
                channel.PublishGameChanges();
            }
        }

        public void OnMaxPlayerReached(GameState gameState)
        {
        }

        public void OnPlayerCountFallBelowMaxPlayer(GameState gameState)
        {
        }

        #endregion
    }
}