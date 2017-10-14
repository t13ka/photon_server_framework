using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events
{
    using YourGame.Common;

    public class UpdateElementsOrderEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ElementsOrderData, IsOptional = false)]
        public byte[] ElementsOrderData ;
    }
}
