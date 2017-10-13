using Photon.SocketServer.Rpc;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.Events
{
    public class QueueEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.Position)]
        public int Position ;
    }
}