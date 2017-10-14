using System.Collections;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.ServerToServer.Operations
{
    public class RegisterGameServerResponse : DataContract
    {
        #region Constructors and Destructors

        public RegisterGameServerResponse(IRpcProtocol protocol, OperationResponse response)
            : base(protocol, response.Parameters)
        {
        }

        public RegisterGameServerResponse()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = 4, IsOptional = true)]
        public Hashtable AuthList ;

        [DataMember(Code = 1, IsOptional = true)]
        public byte[] ExternalAddress ;

        [DataMember(Code = 2)]
        public byte[] InternalAddress ;

        [DataMember(Code = 5, IsOptional = true)]
        public byte[] SharedKey ;

        #endregion
    }
}