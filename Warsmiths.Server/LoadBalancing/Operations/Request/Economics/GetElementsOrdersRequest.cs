using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Economics
{
    public class GetElementsOrdersRequest : Operation
    {
        public GetElementsOrdersRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request)
        {
        }
    }
}
