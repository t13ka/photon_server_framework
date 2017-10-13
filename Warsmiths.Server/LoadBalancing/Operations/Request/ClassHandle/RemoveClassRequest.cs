using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.ClassHandle
{
    public class RemoveClassRequest : Operation
    {
        public RemoveClassRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ClassType, IsOptional = false)]
        public int ClassType ;
    }
}