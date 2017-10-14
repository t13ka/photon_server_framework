using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Logging;
using Photon.SocketServer;
using YourGame.Server.Common;
using YourGame.Server.MasterServer.GameServer;
using YourGame.Server.Operations;
using YourGame.Server.ServerToServer.Events;

namespace YourGame.Server.MasterServer.Lobby
{
    public class GameState
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameState" /> class.
        /// </summary>
        /// <param name="lobby">
        ///     The lobby to which the game belongs.
        /// </param>
        /// <param name="id">
        ///     The game id.
        /// </param>
        /// <param name="maxPlayer">
        ///     The maximum number of player who can join the game.
        /// </param>
        /// <param name="gameServerPeer">
        ///     The game server peer.
        /// </param>
        public GameState(AppLobby lobby, string id, byte maxPlayer, IncomingGameServerPeer gameServerPeer)
        {
            Lobby = lobby;
            Id = id;
            MaxPlayer = maxPlayer;
            IsOpen = true;
            IsVisible = true;
            IsCreatedOnGameServer = false;
            GameServerPlayerCount = 0;
            GameServer = gameServerPeer;
        }

        #endregion

        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public readonly AppLobby Lobby;

        /// <summary>
        ///     Used track peers which currently are joining the game.
        /// </summary>
        private readonly LinkedList<PeerState> _joiningPeers = new LinkedList<PeerState>();

        private readonly LinkedList<string> _userIdList = new LinkedList<string>();

        #endregion

        #region Properties

        public DateTime CreateDateUtc { get; } = DateTime.UtcNow;

        /// <summary>
        ///     Gets the address of the game server on which the game is or should be created.
        /// </summary>
        public IncomingGameServerPeer GameServer { get; }

        /// <summary>
        ///     Gets the number of players who joined the game on the game server.
        /// </summary>
        public int GameServerPlayerCount { get; private set; }

        /// <summary>
        ///     Gets the game id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the game is created on a game server instance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is created on game server; otherwise, <c>false</c>.
        /// </value>
        public bool IsCreatedOnGameServer ;

        /// <summary>
        ///     Gets or sets a value indicating whether the game is open for players to join the game.
        /// </summary>
        /// <value><c>true</c> if the game is open; otherwise, <c>false</c>.</value>
        public bool IsOpen ;

        /// <summary>
        ///     Gets a value indicating whether this instance is visble in the lobby.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is visble in lobby; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisbleInLobby
        {
            get { return IsVisible && IsCreatedOnGameServer; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the game should be visible to other players.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the game is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible ;

        /// <summary>
        ///     Gets the number of players currently joining the game.
        /// </summary>
        public int JoiningPlayerCount
        {
            get { return _joiningPeers.Count; }
        }

        /// <summary>
        ///     Gets or sets the maximum number of player for the game.
        /// </summary>
        public int MaxPlayer ;

        /// <summary>
        ///     Gets the number of players joined the game.
        /// </summary>
        public int PlayerCount
        {
            get { return GameServerPlayerCount + JoiningPlayerCount; }
        }

        public Dictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        #endregion

        #region Public Methods

        public void AddPeer(ILobbyPeer peer)
        {
            var peerState = new PeerState(peer);

            _joiningPeers.AddLast(peerState);

            if (string.IsNullOrEmpty(peerState.UserId) == false)
            {
                _userIdList.AddLast(peer.UserId);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Added peer: gameId={0}, userId={1}, joiningPeers={2}", Id, peer.UserId,
                    _joiningPeers.Count);
            }

            // check if max player is reached and inform the game list
            if (MaxPlayer > 0 && PlayerCount >= MaxPlayer)
            {
                Lobby.GameList.OnMaxPlayerReached(this);
            }

            // update player state in the online players cache
            if (Lobby.Application.PlayerOnlineCache != null && string.IsNullOrEmpty(peer.UserId) == false)
            {
                Lobby.Application.PlayerOnlineCache.OnJoinedGamed(peer.UserId, this);
            }
        }

        public void CheckJoinTimeOuts(DateTime minDateTime)
        {
            var oldPlayerCount = PlayerCount;

            var node = _joiningPeers.First;
            while (node != null)
            {
                var peerState = node.Value;
                var nextNode = node.Next;

                if (peerState.UtcCreated < minDateTime)
                {
                    _joiningPeers.Remove(node);

                    if (string.IsNullOrEmpty(peerState.UserId) == false)
                    {
                        _userIdList.Remove(peerState.UserId);
                        if (Lobby.Application.PlayerOnlineCache != null)
                        {
                            Lobby.Application.PlayerOnlineCache.OnDisconnectFromGameServer(peerState.UserId);
                        }
                    }
                }

                node = nextNode;
            }

            if (MaxPlayer > 0 && oldPlayerCount >= MaxPlayer)
            {
                if (PlayerCount < MaxPlayer)
                {
                    Lobby.GameList.OnPlayerCountFallBelowMaxPlayer(this);
                }
            }
        }

        public string GetServerAddress(ILobbyPeer peer)
        {
            switch (peer.NetworkProtocol)
            {
                case NetworkProtocolType.Udp:
                    return GameServer.UdpAddress;

                case NetworkProtocolType.Tcp:
                    return GameServer.TcpAddress;

                case NetworkProtocolType.WebSocket:
                    return GameServer.WebSocketAddress;

                default:
                    return null;
            }
        }

        public bool MatchGameProperties(Hashtable matchProperties)
        {
            if (matchProperties == null || matchProperties.Count == 0)
            {
                return true;
            }

            foreach (var key in matchProperties.Keys)
            {
                object gameProperty;
                if (!Properties.TryGetValue(key, out gameProperty))
                {
                    return false;
                }

                if (gameProperty.Equals(matchProperties[key]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public Hashtable ToHashTable()
        {
            var h = new Hashtable();
            foreach (var keyValue in Properties)
            {
                h.Add(keyValue.Key, keyValue.Value);
            }

            h[(byte) GameParameter.PlayerCount] = (byte) GameServerPlayerCount;
            h[(byte) GameParameter.MaxPlayer] = (byte) MaxPlayer;
            h[(byte) GameParameter.IsOpen] = IsOpen;
            h.Remove((byte) GameParameter.IsVisible);

            return h;
        }

        public bool TrySetProperties(Hashtable gameProperties, out bool changed, out string debugMessage)
        {
            changed = false;

            byte? maxPlayer;
            bool? isOpen;
            bool? isVisible;
            object[] propertyFilter;

            if (
                !GameParameterReader.TryReadDefaultParameter(gameProperties, out maxPlayer, out isOpen, out isVisible,
                    out propertyFilter, out debugMessage))
            {
                return false;
            }

            if (maxPlayer.HasValue && maxPlayer.Value != MaxPlayer)
            {
                MaxPlayer = maxPlayer.Value;
                Properties[(byte) GameParameter.MaxPlayer] = (byte) MaxPlayer;
                changed = true;
            }

            if (isOpen.HasValue && isOpen.Value != IsOpen)
            {
                IsOpen = isOpen.Value;
                Properties[(byte) GameParameter.IsOpen] = MaxPlayer;
                changed = true;
            }

            if (isVisible.HasValue && isVisible.Value != IsVisible)
            {
                IsVisible = isVisible.Value;
                changed = true;
            }

            Properties.Clear();
            foreach (DictionaryEntry entry in gameProperties)
            {
                if (entry.Value != null)
                {
                    Properties[entry.Key] = entry.Value;
                }
            }

            debugMessage = string.Empty;
            return true;
        }

        public bool Update(UpdateGameEvent updateOperation)
        {
            var changed = false;

            if (IsCreatedOnGameServer == false)
            {
                IsCreatedOnGameServer = true;
                changed = true;
            }

            if (GameServerPlayerCount != updateOperation.ActorCount)
            {
                GameServerPlayerCount = updateOperation.ActorCount;
                changed = true;
            }

            if (updateOperation.NewUsers != null)
            {
                foreach (var userId in updateOperation.NewUsers)
                {
                    OnPeerJoinedGameServer(userId);
                }
            }

            if (updateOperation.RemovedUsers != null)
            {
                foreach (var userId in updateOperation.RemovedUsers)
                {
                    OnPeerLeftGameServer(userId);
                }
            }

            if (updateOperation.MaxPlayers.HasValue && updateOperation.MaxPlayers.Value != MaxPlayer)
            {
                MaxPlayer = updateOperation.MaxPlayers.Value;
                Properties[(byte) GameParameter.MaxPlayer] = MaxPlayer;
                changed = true;
            }

            if (updateOperation.IsOpen.HasValue && updateOperation.IsOpen.Value != IsOpen)
            {
                IsOpen = updateOperation.IsOpen.Value;
                Properties[(byte) GameParameter.IsOpen] = MaxPlayer;
                changed = true;
            }

            if (updateOperation.IsVisible.HasValue && updateOperation.IsVisible.Value != IsVisible)
            {
                IsVisible = updateOperation.IsVisible.Value;
                changed = true;
            }

            if (updateOperation.PropertyFilter != null)
            {
                var lobbyProperties = new HashSet<object>(updateOperation.PropertyFilter);

                var keys = new object[Properties.Keys.Count];
                Properties.Keys.CopyTo(keys, 0);

                foreach (var key in keys)
                {
                    if (lobbyProperties.Contains(key) == false)
                    {
                        Properties.Remove(key);
                        changed = true;
                    }
                }

                // add max players even if it's not in the property filter
                // MaxPlayer is always reported to the client and available 
                // for JoinRandom matchmaking
                Properties[(byte) GameParameter.MaxPlayer] = (byte) MaxPlayer;
            }

            if (updateOperation.GameProperties != null)
            {
                changed |= UpdateProperties(updateOperation.GameProperties);
            }

            return changed;
        }

        public void OnRemoved()
        {
            if (Lobby.Application.PlayerOnlineCache != null && _userIdList.Count > 0)
            {
                foreach (var playerId in _userIdList)
                {
                    Lobby.Application.PlayerOnlineCache.OnDisconnectFromGameServer(playerId);
                }
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "GameState {0}: Lobby: {9}, PlayerCount: {1}, Created on GS: {2} at {3}, GSPlayerCount: {4}, IsOpen: {5}, IsVisibleInLobby: {6}, IsVisible: {7}, UtcCreated: {8}",
                    Id,
                    PlayerCount,
                    IsCreatedOnGameServer,
                    GameServer != null ? GameServer.ToString() : string.Empty,
                    GameServerPlayerCount,
                    IsOpen,
                    IsVisbleInLobby,
                    IsVisible,
                    CreateDateUtc,
                    Lobby.LobbyName);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Invoked for peers which has joined the game on the game server instance.
        /// </summary>
        /// <param name="userId">The user id of the peer joined.</param>
        private void OnPeerJoinedGameServer(string userId)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("User joined on game server: gameId={0}, userId={1}", Id, userId);
            }

            // remove the peer from the joining list
            var removed = RemoveFromJoiningList(userId);
            if (removed == false && Log.IsDebugEnabled)
            {
                Log.DebugFormat("User not found in joining list: gameId={0}, userId={1}", Id, userId);
            }

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // update player state in the online players cache
            if (Lobby.Application.PlayerOnlineCache != null)
            {
                Lobby.Application.PlayerOnlineCache.OnJoinedGamed(userId, this);
            }
        }

        /// <summary>
        ///     Invoked for peers which has left the game on the game server instance.
        /// </summary>
        /// <param name="userId">The user id of the peer left.</param>
        private void OnPeerLeftGameServer(string userId)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("User left on game server: gameId={0}, userId={1}", Id, userId);
            }

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            _userIdList.Remove(userId);

            // update player state in the online players cache
            if (Lobby.Application.PlayerOnlineCache != null)
            {
                Lobby.Application.PlayerOnlineCache.OnDisconnectFromGameServer(userId);
            }
        }

        /// <summary>
        ///     Removes a peer with the specified user id from the list of joining peers.
        /// </summary>
        /// <param name="userId">The user id of the peer to remove</param>
        /// <returns>True if the peer has been removed; otherwise false.</returns>
        private bool RemoveFromJoiningList(string userId)
        {
            if (userId == null)
            {
                userId = string.Empty;
            }

            var node = _joiningPeers.First;

            while (node != null)
            {
                if (node.Value.UserId == userId)
                {
                    _joiningPeers.Remove(node);
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        private bool UpdateProperties(Hashtable props)
        {
            var changed = false;

            foreach (DictionaryEntry entry in props)
            {
                object oldValue;

                if (Properties.TryGetValue(entry.Key, out oldValue))
                {
                    if (entry.Value == null)
                    {
                        changed = true;
                        Properties.Remove(entry.Key);
                    }
                    else
                    {
                        if (oldValue == null || !oldValue.Equals(entry.Value))
                        {
                            changed = true;
                            Properties[entry.Key] = entry.Value;
                        }
                    }
                }
                else
                {
                    if (entry.Value != null)
                    {
                        changed = true;
                        Properties[entry.Key] = entry.Value;
                    }
                }
            }

            return changed;
        }

        #endregion
    }
}