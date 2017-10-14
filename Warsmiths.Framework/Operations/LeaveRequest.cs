namespace YourGame.Server.Framework.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class LeaveRequest : Operation
    {
        public LeaveRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
    }
}