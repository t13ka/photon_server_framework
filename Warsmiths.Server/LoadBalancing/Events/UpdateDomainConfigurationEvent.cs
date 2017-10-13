using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateDomainConfigurationEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.DomainConfiguration, IsOptional = false)]
        public byte[] DomainConfiguration ;
    }
}
