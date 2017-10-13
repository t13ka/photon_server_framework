using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.ClassHandle
{
    public class AddClassRequest : Operation
    {
        public AddClassRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ClassType, IsOptional = false)]
        public int ClassType ;
    }
}