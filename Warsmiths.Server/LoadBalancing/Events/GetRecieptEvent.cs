using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class GetRecieptEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptData, IsOptional = false)]
        public byte[] RecieptData ;
    }
}