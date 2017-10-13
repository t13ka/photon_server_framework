using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateElementsOrderEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ElementsOrderData, IsOptional = false)]
        public byte[] ElementsOrderData ;
    }
}
