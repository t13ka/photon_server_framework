namespace Warsmiths.Server.Operations.Request.GameManagement
{
    using System.Collections;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    using Warsmiths.Server.Framework.Operations;

    public class JoinLobbyRequest : Operation
    {
        public JoinLobbyRequest()
        {
        }

        public JoinLobbyRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)ParameterCode.GameCount, IsOptional = true)]
        public int GameListCount ;

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties ;

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyName ;

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType ;
    }
}
