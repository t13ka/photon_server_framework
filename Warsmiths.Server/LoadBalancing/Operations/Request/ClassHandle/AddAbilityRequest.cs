using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.ClassHandle
{
    public class AddAbilityRequest : Operation
    {
        public AddAbilityRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Name, IsOptional = false)]
        public string AblilityName ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Position, IsOptional = false)]
        public int Position ;
    }
}