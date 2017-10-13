using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Equipment
{
    public class WearEquipmentResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EquipmentId, IsOptional = false)]
        public string EquipmentId ;
    }
}
