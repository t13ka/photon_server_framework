using System.Collections;
using Photon.SocketServer.Rpc;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.Events
{
    public class GameListEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.GameList)]
        public Hashtable Data ;
    }
}