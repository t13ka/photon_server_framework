namespace YourGame.Server.Framework.Operations
{
    using System.Collections;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class SetPropertiesRequest : Operation
    {
        public SetPropertiesRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public SetPropertiesRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.ActorNr, IsOptional = true)]
        public int ActorNumber;

        [DataMember(Code = (byte)ParameterKey.Broadcast, IsOptional = true)]
        public bool Broadcast;

        [DataMember(Code = (byte)ParameterKey.Properties)]
        public Hashtable Properties;
    }
}