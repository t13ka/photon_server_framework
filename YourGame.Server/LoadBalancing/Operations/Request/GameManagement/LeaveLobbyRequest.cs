namespace YourGame.Server.Operations.Request.GameManagement
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class LeaveLobbyRequest : Operation
    {
        public LeaveLobbyRequest(IRpcProtocol protocol, OperationRequest request) : base(protocol, request)
        {
        }
    }
}