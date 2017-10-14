using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    using YourGame.Common;

    public class CharacterProfileEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.Name, IsOptional = false)]
        public string CharacterName ;

        [DataMember(Code = (byte)ParameterCode.ProfileData, IsOptional = false)]
        public string CharacterProfileData ;
    }
}
