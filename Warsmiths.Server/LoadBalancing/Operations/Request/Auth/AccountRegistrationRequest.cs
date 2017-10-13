using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Auth
{
    public class AccountRegistrationRequest : Operation
    {
        public AccountRegistrationRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.UserFirstName, IsOptional = false)]
        public string UserFirstName ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.UserLastName, IsOptional = false)]
        public string UserLastName ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.LoginReg, IsOptional = false)]
        public string LoginReg ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Password, IsOptional = false)]
        public string Md5Password ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Email, IsOptional = false)]
        public string Email ;
    }
}
