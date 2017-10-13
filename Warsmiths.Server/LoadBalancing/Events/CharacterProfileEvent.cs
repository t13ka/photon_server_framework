using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class CharacterProfileEvent : DataContract
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Name, IsOptional = false)]
        public string CharacterName ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ProfileData, IsOptional = false)]
        public string CharacterProfileData ;
    }
}
