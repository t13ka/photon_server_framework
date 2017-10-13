using System.Collections;

using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Framework.Operations
{
    public class JoinResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterKey.ActorNr)]
        public int ActorNr;

        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable CurrentActorProperties;

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable CurrentGameProperties;

        #endregion
    }
}