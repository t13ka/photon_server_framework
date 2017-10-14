using System.Collections;
using Photon.SocketServer.Rpc;
using YourGame.Server.Operations;

namespace YourGame.Server.Events
{
    public class GameListUpdateEvent 
    {
        [DataMember(Code = (byte)ParameterCode.GameList)]
        public Hashtable Data ;
    }
}