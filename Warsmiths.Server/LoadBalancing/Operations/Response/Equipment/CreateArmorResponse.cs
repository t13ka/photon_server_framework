using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Equipment
{
    public class CreateArmorResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EquipmentId, IsOptional = false)]
        public string EquipmentId ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptId, IsOptional = false)]
        public string RecieptId ;
    }
}
