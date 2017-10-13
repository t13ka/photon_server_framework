using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Framework.Operations
{
    public class LeaveRequest : Operation
    {
        public LeaveRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
    }
}