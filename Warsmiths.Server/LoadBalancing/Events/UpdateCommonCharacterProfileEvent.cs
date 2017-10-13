using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events 
{
    public class UpdateCommonCharacterProfileEvent : DataContract
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CommonCharacterProfile, IsOptional = false)]
        public byte[] CommonCharacterProfile ;
    }
}
