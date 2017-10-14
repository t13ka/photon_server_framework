using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.ServerToServer.Events
{
    public class UpdateServerEvent : DataContract
    {
        #region Constructors and Destructors

        public UpdateServerEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        public UpdateServerEvent()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ServerParameterCode.LoadIndex, IsOptional = false)]
        public byte LoadIndex ;

        [DataMember(Code = (byte)ServerParameterCode.ServerState, IsOptional = true)]
        public int State ;

        [DataMember(Code = (byte)ServerParameterCode.PeerCount, IsOptional = false)]
        public int PeerCount ;

        [DataMember(Code = (byte)ServerParameterCode.GameCount, IsOptional = true)]
        public int GameCount ;

        #endregion
    }
}