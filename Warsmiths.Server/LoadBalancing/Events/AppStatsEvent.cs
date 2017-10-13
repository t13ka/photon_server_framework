using Photon.SocketServer.Rpc;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.Events
{
    public class AppStatsEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.MasterPeerCount)]
        public int MasterPeerCount ;

        [DataMember(Code = (byte)ParameterCode.PeerCount)]
        public int PlayerCount ;

        [DataMember(Code = (byte)ParameterCode.GameCount)]
        public int GameCount ;
    }
}