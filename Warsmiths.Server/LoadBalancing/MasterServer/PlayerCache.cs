using System;
using System.Collections.Generic;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Server.MasterServer.Lobby;
using Warsmiths.Server.Operations.Request.Social;
using Warsmiths.Server.Operations.Response;

namespace Warsmiths.Server.MasterServer
{
    public class PlayerCache : IDisposable
    {
        /// <summary>
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly PoolFiber _fiber = new PoolFiber();

        /// <summary>
        /// </summary>
        private readonly Dictionary<string, PlayerState> _playerDict = new Dictionary<string, PlayerState>();

        public PlayerCache()
        {
            _fiber.Start();
        }

        public void Dispose()
        {
            var poolFiber = _fiber;
            if (poolFiber != null)
            {
                poolFiber.Dispose();
            }
        }

        public void OnConnectedToMaster(string playerId)
        {
            _fiber.Enqueue(() => HandleOnConnectedToMaster(playerId));
        }

        public void OnDisconnectFromMaster(string playerId)
        {
            _fiber.Enqueue(() => HandleOnDisconnectFromMaster(playerId));
        }

        public void OnDisconnectFromGameServer(string playerId)
        {
            _fiber.Enqueue(() => HandleOnDisconnectFromGameServer(playerId));
        }

        public void OnJoinedGamed(string playerId, GameState gameState)
        {
            _fiber.Enqueue(() => HandleOnJoinedGamed(playerId, gameState));
        }

        public void FiendFriends(PeerBase peer, FindFriendsRequest request, SendParameters sendParameters)
        {
            _fiber.Enqueue(() => HandleFiendFriends(peer, request, sendParameters));
        }

        private void HandleOnConnectedToMaster(string playerId)
        {
            try
            {
                // only peers with userid set can be handled
                if (string.IsNullOrEmpty(playerId))
                {
                    return;
                }

                PlayerState playerState;
                if (_playerDict.TryGetValue(playerId, out playerState) == false)
                {
                    playerState = new PlayerState(playerId);
                    _playerDict.Add(playerId, playerState);
                }

                playerState.IsConnectedToMaster = true;

                if (Log.IsDebugEnabled)
                {
                    var gameId = playerState.Game == null ? string.Empty : playerState.Game.Id;
                    Log.DebugFormat("Player state changed: pid={0}, master={1}, gid={2}", playerId,
                        playerState.IsConnectedToMaster, gameId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleOnDisconnectFromMaster(string playerId)
        {
            try
            {
                // only peers with userid set can be handled
                if (string.IsNullOrEmpty(playerId))
                {
                    return;
                }
                
                PlayerState playerState;
                if (_playerDict.TryGetValue(playerId, out playerState) == false)
                {
                    return;
                }

                playerState.IsConnectedToMaster = false;
                if (playerState.Game != null)
                {
                    return;
                }

                _playerDict.Remove(playerId);
                if (Log.IsDebugEnabled)
                {
                    var gameId = playerState.Game == null ? string.Empty : playerState.Game.Id;
                    Log.DebugFormat("Player removed: pid={0}, master={1}, gid={2}", playerId,
                        playerState.IsConnectedToMaster, gameId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleOnJoinedGamed(string playerId, GameState gameState)
        {
            try
            {
                // only peers with userid set can be handled
                if (string.IsNullOrEmpty(playerId))
                {
                    return;
                }

                PlayerState playerState;
                if (_playerDict.TryGetValue(playerId, out playerState) == false)
                {
                    playerState = new PlayerState(playerId);
                    _playerDict.Add(playerId, playerState);
                }

                playerState.Game = gameState;

                if (Log.IsDebugEnabled)
                {
                    var gameId = gameState == null ? string.Empty : gameState.Id;
                    Log.DebugFormat("Player state changed: pid={0}, master={1}, gid={2}", playerId,
                        playerState.IsConnectedToMaster, gameId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleOnDisconnectFromGameServer(string playerId)
        {
            try
            {
                // only peers with userid set can be handled
                if (string.IsNullOrEmpty(playerId))
                {
                    return;
                }

                PlayerState playerState;
                if (_playerDict.TryGetValue(playerId, out playerState) == false)
                {
                    return;
                }

                playerState.Game = null;
                if (playerState.IsConnectedToMaster)
                {
                    return;
                }

                _playerDict.Remove(playerId);
                if (Log.IsDebugEnabled)
                {
                    var gameId = playerState.Game == null ? string.Empty : playerState.Game.Id;
                    Log.DebugFormat("Player removed: pid={0}, master={1}, gid={2}", playerId,
                        playerState.IsConnectedToMaster, gameId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HandleFiendFriends(PeerBase peer, FindFriendsRequest request, SendParameters sendParameters)
        {
            try
            {
                var onlineList = new bool[request.UserList.Length];
                var gameIds = new string[request.UserList.Length];

                for (var i = 0; i < request.UserList.Length; i++)
                {
                    PlayerState playerState;
                    if (_playerDict.TryGetValue(request.UserList[i], out playerState))
                    {
                        onlineList[i] = true;
                        if (playerState.Game != null)
                        {
                            gameIds[i] = playerState.Game.Id;
                        }
                        else
                        {
                            gameIds[i] = string.Empty;
                        }
                    }
                    else
                    {
                        gameIds[i] = string.Empty;
                    }
                }

                var response = new FindFriendsResponse {IsOnline = onlineList, UserStates = gameIds};
                var opResponse = new OperationResponse((byte) OperationCode.FiendFriends, response);
                peer.SendOperationResponse(opResponse, sendParameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}