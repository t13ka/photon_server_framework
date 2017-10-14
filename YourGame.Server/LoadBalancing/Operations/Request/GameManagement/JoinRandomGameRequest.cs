namespace YourGame.Server.Operations.Request.GameManagement
{
    using System.Collections;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

    public class JoinRandomGameRequest : Operation
    {
        public JoinRandomGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public JoinRandomGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties ;

        [DataMember(Code = (byte)ParameterCode.Position, IsOptional = true)]
        public byte JoinRandomType ;

        [DataMember(Code = (byte)ParameterKey.Data, IsOptional = true)]
        public string QueryData ;

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyName ;

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType ;
    }
}