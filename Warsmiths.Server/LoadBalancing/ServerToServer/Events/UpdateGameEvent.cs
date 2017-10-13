using System.Collections;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Warsmiths.Server.Framework.Operations;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.ServerToServer.Events
{
    public class UpdateGameEvent : DataContract
    {
        #region Constructors and Destructors

        public UpdateGameEvent()
        {
        }

        public UpdateGameEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterCode.PeerCount, IsOptional = true)]
        public byte ActorCount ;

        [DataMember(Code = (byte)ParameterCode.ApplicationId, IsOptional = true)]
        public string ApplicationId ;

        [DataMember(Code = (byte)ParameterCode.AppVersion, IsOptional = true)]
        public string ApplicationVersion ;

        [DataMember(Code = (byte)ParameterCode.GameId, IsOptional = false)]
        public string GameId ;

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyId ;

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType ;

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties ;

        [DataMember(Code = (byte)ServerParameterCode.NewUsers, IsOptional = true)]
        public string[] NewUsers ;

        [DataMember(Code = (byte)ServerParameterCode.RemovedUsers, IsOptional = true)]
        public string[] RemovedUsers ;

        [DataMember(Code = (byte)ServerParameterCode.Reinitialize, IsOptional = true)]
        public bool Reinitialize ;

        [DataMember(Code = (byte)ServerParameterCode.MaxPlayer, IsOptional = true)]
        public byte? MaxPlayers ;

        [DataMember(Code = (byte)ServerParameterCode.IsOpen, IsOptional = true)]
        public bool? IsOpen ;

        [DataMember(Code = (byte)ServerParameterCode.IsVisible, IsOptional = true)]
        public bool? IsVisible ;

        [DataMember(Code = (byte)ServerParameterCode.LobbyPropertyFilter, IsOptional = true)]
        public object[] PropertyFilter ;

        #endregion
    }
}