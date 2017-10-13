using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Equipment.Modules
{
    public class RemoveModuleRequest : Operation
    {
        public RemoveModuleRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ModuleId, IsOptional = false)]
        public string ModuleId ;
    }
}