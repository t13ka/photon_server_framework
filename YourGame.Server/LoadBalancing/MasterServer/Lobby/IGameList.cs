using System;
using System.Collections;
using Photon.SocketServer;

using YourGame.Server.MasterServer.GameServer;
using YourGame.Server.Operations;
using YourGame.Server.ServerToServer.Events;

namespace YourGame.Server.MasterServer.Lobby
{
    using YourGame.Common;

    public interface IGameList
    {
        int Count { get; }

        void AddGameState(GameState gameState);

        int CheckJoinTimeOuts(TimeSpan timeOut);

        int CheckJoinTimeOuts(DateTime minDateTime);

        bool ContainsGameId(string gameId);

        IGameListSubscibtion AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount);

        void RemoveGameServer(IncomingGameServerPeer gameServer);

        bool RemoveGameState(string gameId);

        bool TryGetGame(string gameId, out GameState gameState);

        ErrorCode TryGetRandomGame(JoinRandomType joinType, ILobbyPeer peer, Hashtable gameProperties, string query, out GameState gameState, out string message);

        bool UpdateGameState(UpdateGameEvent updateOperation, IncomingGameServerPeer gameServerPeer, out GameState gameState);

        void PublishGameChanges();

        void OnMaxPlayerReached(GameState gameState);

        void OnPlayerCountFallBelowMaxPlayer(GameState gameState);
    }
}
