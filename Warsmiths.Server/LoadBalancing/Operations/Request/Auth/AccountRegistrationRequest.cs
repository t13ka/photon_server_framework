using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Auth
{
    using YourGame.Common;

    public class AccountRegistrationRequest : Operation
    {
        public AccountRegistrationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)ParameterCode.UserFirstName, IsOptional = false)]
        public string UserFirstName ;

        [DataMember(Code = (byte)ParameterCode.UserLastName, IsOptional = false)]
        public string UserLastName ;

        [DataMember(Code = (byte)ParameterCode.LoginReg, IsOptional = false)]
        public string LoginReg ;

        [DataMember(Code = (byte)ParameterCode.Password, IsOptional = false)]
        public string Md5Password ;

        [DataMember(Code = (byte)ParameterCode.Email, IsOptional = false)]
        public string Email ;
    }
}
