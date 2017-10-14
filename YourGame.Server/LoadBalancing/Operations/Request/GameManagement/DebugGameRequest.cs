namespace YourGame.Server.Operations.Request.GameManagement
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class DebugGameRequest : Operation
    {
        public DebugGameRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        public DebugGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterCode.GameId, IsOptional = false)]
        public string GameId ;

        [DataMember(Code = (byte)ParameterCode.Info, IsOptional = true)]
        public string Info ;
    }
}
