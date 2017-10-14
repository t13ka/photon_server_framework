using YourGame.Server.MasterServer.Lobby;

namespace YourGame.Server.MasterServer
{
    public class PlayerState
    {
        public readonly string PlayerId;

        public PlayerState(string playerId)
        {
            PlayerId = playerId;
        }

        public bool IsConnectedToMaster ;
        public GameState Game ;
    }
}