using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using YourGame.Server.Operations;

namespace YourGame.Server.ServerToServer.Events
{
    public class RemoveGameEvent : DataContract
    {
        #region Constructors and Destructors

        public RemoveGameEvent()
        {
        }

        public RemoveGameEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterCode.ApplicationId, IsOptional = true)]
        public string ApplicationId ;

        [DataMember(Code = (byte)ParameterCode.AppVersion, IsOptional = true)]
        public string ApplicationVersion ;

        [DataMember(Code = (byte)ParameterCode.GameId, IsOptional = false)]
        public string GameId ;

        #endregion
    }
}