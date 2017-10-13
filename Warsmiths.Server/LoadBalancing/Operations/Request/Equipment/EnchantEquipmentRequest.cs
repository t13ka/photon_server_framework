using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Equipment
{
    public class EnchantEquipmentRequest : Operation
    {
        public EnchantEquipmentRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.EquipmentId, IsOptional = false)]
        public string EquipmentId ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EnchantValue, IsOptional = false)]
        public int EnchantValue ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ElementId, IsOptional = false)]
        public string ElementId ;
    }
}