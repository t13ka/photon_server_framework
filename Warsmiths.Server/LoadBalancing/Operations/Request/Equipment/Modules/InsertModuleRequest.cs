using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Equipment.Modules
{
    public class InsertModuleRequest : Operation
    {
        public InsertModuleRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ModuleId, IsOptional = false)]
        public string ModuleId ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ArmortPartType, IsOptional = false)]
        public int ArmorPartType ;
    }
}
