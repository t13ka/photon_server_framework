using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Request.Auth
{
    public class AuthenticateRequest : Operation
    {
        #region Constructors and Destructors

        public AuthenticateRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public AuthenticateRequest()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterCode.ApplicationId, IsOptional = true)]
        public string ApplicationId ;

        [DataMember(Code = (byte)ParameterCode.AppVersion, IsOptional = true)]
        public string ApplicationVersion ;

        [DataMember(Code = (byte)ParameterCode.Secret, IsOptional = true)]
        public string Secret ;

        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = true)]
        public string UserId ;

        [DataMember(Code = (byte)ParameterCode.ClientAuthenticationType, IsOptional = true)]
        public byte ClientAuthenticationType ;

        [DataMember(Code = (byte)ParameterCode.ClientAuthenticationParams, IsOptional = true)]
        public string ClientAuthenticationParams ;

        [DataMember(Code = (byte)ParameterCode.ClientAuthenticationData, IsOptional = true)]
        public object ClientAuthenticationData ;

        #endregion
    }
}