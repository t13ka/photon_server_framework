using System.Collections;

using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Framework.Operations
{
    public class GetPropertiesResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable ActorProperties;

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties;

        #endregion
    }
}