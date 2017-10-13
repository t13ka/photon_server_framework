using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.ServerToServer.Events
{
    public class UpdateAppStatsEvent : DataContract
    {
        public UpdateAppStatsEvent()
        {
        }

        public UpdateAppStatsEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        [DataMember(Code = 0, IsOptional = false)]
        public int GameCount ;

        [DataMember(Code = 1, IsOptional = false)]
        public int PlayerCount ;
    }
}