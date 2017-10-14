namespace YourGame.Server.Framework.Operations
{
    using System.Collections;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class JoinRequest : Operation
    {
        public JoinRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public JoinRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable ActorProperties;

        [DataMember(Code = (byte)ParameterKey.Broadcast, IsOptional = true)]
        public bool BroadcastActorProperties;

        [DataMember(Code = (byte)ParameterKey.GameId)]
        public string GameId;

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties;

        [DataMember(Code = (byte)ParameterKey.DeleteCacheOnLeave, IsOptional = true)]
        public bool DeleteCacheOnLeave;

        [DataMember(Code = (byte)ParameterKey.SuppressRoomEvents, IsOptional = true)]
        public bool SuppressRoomEvents;

        [DataMember(Code = (byte)ParameterKey.ActorNr, IsOptional = true)]
        public int ActorNr;
    }
}