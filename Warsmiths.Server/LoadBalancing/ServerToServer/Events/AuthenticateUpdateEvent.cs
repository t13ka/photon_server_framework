using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.ServerToServer.Events
{
    public class AuthenticateUpdateEvent : DataContract
    {
        #region Constructors and Destructors

        public AuthenticateUpdateEvent()
        {
        }

        public AuthenticateUpdateEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterCode.ApplicationId, IsOptional = false)]
        public string ApplicationId ;

        [DataMember(Code = (byte)ParameterCode.GameList, IsOptional = false)]
        public bool Data ;

        [DataMember(Code = 1, IsOptional = true)]
        public bool IsClientAuthenticationRequired ;

        [DataMember(Code = 2, IsOptional = true)]
        public bool IsClientAuthenticationEnabled ;

        #endregion
    }
}