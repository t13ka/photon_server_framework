using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events
{
    using YourGame.Common;

    public class UpdateEquipmentEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.EquipmentData, IsOptional = false)]
        public byte[] EquipmentData ;
    }
}
