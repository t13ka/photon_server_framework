using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events
{
    public class GameServerOfflineEvent : DataContract
    {
        [DataMember(Code = 0, IsOptional = true)]
        public int TimeLeft ;
    }
}
