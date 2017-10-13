using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Characters
{
    public class SelectCharacterRequest : Operation
    {
        public SelectCharacterRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.Name, IsOptional = false)]
        public string Name ;
    }
}