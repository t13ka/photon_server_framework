using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.ClassHandle
{
    public class RemoveAbilityRequest : Operation
    {
        public RemoveAbilityRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Name, IsOptional = false)]
        public string AblilityName ;
    }
}