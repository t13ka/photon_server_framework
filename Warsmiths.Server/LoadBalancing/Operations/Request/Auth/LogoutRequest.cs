using Photon.SocketServer;
using Photon.SocketServer.Rpc;


namespace Warsmiths.Server.Operations.Request.Auth
{
    public class LogoutRequest : Operation
    {
        public LogoutRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
    }
}
