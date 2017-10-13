using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateEquipmentEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EquipmentData, IsOptional = false)]
        public byte[] EquipmentData ;
    }
}
