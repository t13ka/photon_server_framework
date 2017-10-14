using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Request.Auction
{
    public class GetLotsRequest : Operation
    {
        public GetLotsRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
    }
}
