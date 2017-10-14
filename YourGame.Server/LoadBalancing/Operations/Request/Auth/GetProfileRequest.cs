using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Request.Auth
{
    public class GetProfileRequest : Operation
    {
        public GetProfileRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
    }
}