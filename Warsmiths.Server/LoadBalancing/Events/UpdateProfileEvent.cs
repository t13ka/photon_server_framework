using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateProfileEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ProfileData, IsOptional = false)]
        public byte[] ProfileData ;
    }
}
