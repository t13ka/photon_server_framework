using Photon.SocketServer.Rpc;


namespace Warsmiths.Server.Operations.Response.Equipment
{
    public class DestroyEquipmentResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.DestroyedEquipmentDataResult, IsOptional = false)]
        public byte[] DestroyedEquipmentData ;
    }
}
