using System.Collections.Generic;
using ExitGames.Logging;
using Warsmiths.Server.MasterServer.GameServer;

namespace Warsmiths.Server.MasterServer.Lobby
{
    public class LobbyFactory
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly GameApplication _application;
        private readonly AppLobby _defaultLobby;

        private readonly Dictionary<KeyValuePair<string, AppLobbyType>, AppLobby> _lobbyDict =
            new Dictionary<KeyValuePair<string, AppLobbyType>, AppLobby>();

        public LobbyFactory(GameApplication application)
        {
            _application = application;
            _defaultLobby = new AppLobby(_application, string.Empty, AppLobbyType.Default);
        }

        public LobbyFactory(GameApplication application, AppLobbyType defaultLobbyType)
        {
            _application = application;
            _defaultLobby = new AppLobby(_application, string.Empty, defaultLobbyType);
        }

        public bool GetOrCreateAppLobby(string lobbyName, AppLobbyType lobbyType, out AppLobby lobby)
        {
            if (string.IsNullOrEmpty(lobbyName))
            {
                lobby = _defaultLobby;
                return true;
            }

            var key = new KeyValuePair<string, AppLobbyType>(lobbyName, lobbyType);

            lock (_lobbyDict)
            {
                if (_lobbyDict.TryGetValue(key, out lobby))
                {
                    return true;
                }

                lobby = new AppLobby(_application, lobbyName, lobbyType);
                _lobbyDict.Add(key, lobby);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Created lobby: name={0}, type={1}", lobbyName, lobbyType);
            }

            return true;
        }

        public void OnGameServerRemoved(IncomingGameServerPeer gameServerPeer)
        {
            _defaultLobby.RemoveGameServer(gameServerPeer);

            lock (_lobbyDict)
            {
                foreach (var lobby in _lobbyDict.Values)
                {
                    lobby.RemoveGameServer(gameServerPeer);
                }
            }
        }
    }
}