using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events 
{
    using YourGame.Common;

    public class UpdateCommonCharacterProfileEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.CommonCharacterProfile, IsOptional = false)]
        public byte[] CommonCharacterProfile ;
    }
}
