using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Auth
{
    using YourGame.Common;

    public class LoginRequest : Operation
    {
        public LoginRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)ParameterCode.LoginReg, IsOptional = false)]
        public string LoginReg ;

        [DataMember(Code = (byte)ParameterCode.Password, IsOptional = false)]
        public string Password ;
    }
}
