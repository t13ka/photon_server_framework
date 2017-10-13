using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Characters
{
    public class SetExperienceRequest : Operation
    {
        public SetExperienceRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Experience, IsOptional = false)]
        public int Experience ;
    }
}