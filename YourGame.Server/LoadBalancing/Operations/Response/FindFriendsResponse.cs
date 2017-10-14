using System.Collections.Generic;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Response
{
    public class FindFriendsResponse : DataContract
    {
        public FindFriendsResponse()
        {
        }

        public FindFriendsResponse(IRpcProtocol protocol, IDictionary<byte, object> parameter)
            : base(protocol, parameter)
        {
        }

        [DataMember(Code = 1, IsOptional = true)]
        public bool[] IsOnline ;

        [DataMember(Code = 2, IsOptional = true)]
        public string[] UserStates ;
    }
}
