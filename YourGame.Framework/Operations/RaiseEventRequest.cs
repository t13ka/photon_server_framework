namespace YourGame.Server.Framework.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class RaiseEventRequest : Operation
    {
        #region Constructors and Destructors

        public RaiseEventRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public RaiseEventRequest()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterKey.Actors, IsOptional = true)]
        public int[] Actors;

        [DataMember(Code = (byte)ParameterKey.Cache, IsOptional = true)]
        public byte Cache;

        [DataMember(Code = (byte)ParameterKey.Data, IsOptional = true)]
        public object Data;

        [DataMember(Code = (byte)ParameterKey.Code, IsOptional = true)]
        public byte EvCode;

        [DataMember(Code = (byte)ParameterKey.Flush, IsOptional = true)]
        public bool Flush;

        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = true)]
        public string GameId;

        [DataMember(Code = (byte)ParameterKey.ReceiverGroup, IsOptional = true)]
        public byte ReceiverGroup;

        [DataMember(Code = (byte)ParameterKey.Group, IsOptional = true)]
        public byte Group;

        #endregion
    }
}