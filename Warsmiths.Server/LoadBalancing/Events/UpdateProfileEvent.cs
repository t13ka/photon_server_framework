using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    using YourGame.Common;

    public class UpdateProfileEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ProfileData, IsOptional = false)]
        public byte[] ProfileData ;
    }
}
