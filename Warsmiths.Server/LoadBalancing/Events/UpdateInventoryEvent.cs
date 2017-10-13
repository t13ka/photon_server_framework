using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateInventoryEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.InventoryData, IsOptional = false)]
        public byte[] InventoryData ;
    }
}
