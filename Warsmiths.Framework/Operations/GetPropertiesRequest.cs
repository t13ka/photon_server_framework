namespace YourGame.Server.Framework.Operations
{
    using System.Collections;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class GetPropertiesRequest : Operation
    {
        #region Constructors and Destructors

        public GetPropertiesRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public GetPropertiesRequest()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterKey.Actors, IsOptional = true)]
        public int[] ActorNumbers;

        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public IList ActorPropertyKeys;

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public IList GamePropertyKeys;

        [DataMember(Code = (byte)ParameterKey.Properties, IsOptional = true)]
        public byte PropertyType;

        #endregion
    }
}