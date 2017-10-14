using Photon.SocketServer.Rpc;
using YourGame.Server.Operations;

namespace YourGame.Server.Events
{
    public class QueueEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.Position)]
        public int Position ;
    }
}