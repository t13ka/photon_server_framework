using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class NotificationEvent : DataContract
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.NotificationData, IsOptional = false)]
        public byte[] NotificationData ;
    }
}
