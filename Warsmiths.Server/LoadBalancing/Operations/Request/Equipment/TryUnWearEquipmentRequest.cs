using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Server.Operations.Request.Equipment
{
    public class TryUnwearEquipmentRequest : Operation
    {
        public TryUnwearEquipmentRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EquipmentId, IsOptional = false)]
        public string EquipmentId ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EquipmentPlace, IsOptional = false)]
        public int EquipmentPlace ;
    }
}
