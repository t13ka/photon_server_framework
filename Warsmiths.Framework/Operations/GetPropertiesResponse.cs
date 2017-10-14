namespace YourGame.Server.Framework.Operations
{
    using System.Collections;

    using Photon.SocketServer.Rpc;

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