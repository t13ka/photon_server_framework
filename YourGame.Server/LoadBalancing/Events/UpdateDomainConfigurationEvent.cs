using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events
{
    using YourGame.Common;

    public class UpdateDomainConfigurationEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.DomainConfiguration, IsOptional = false)]
        public byte[] DomainConfiguration ;
    }
}
