using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.GamaManagement
{
    public class LeaveLobbyRequest : Operation
    {
        public LeaveLobbyRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request)
        {
        }
    }
}