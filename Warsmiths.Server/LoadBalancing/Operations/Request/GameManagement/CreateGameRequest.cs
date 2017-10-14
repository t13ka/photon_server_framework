namespace Warsmiths.Server.Operations.Request.GameManagement
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

    public class CreateGameRequest : JoinRequest
    {
        public CreateGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public CreateGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = true)]
        public new string GameId ;

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyName ;

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType ;
    }
}