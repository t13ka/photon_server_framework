namespace YourGame.Server.Framework.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class ChangeGroups : Operation
    {
        public ChangeGroups(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)ParameterKey.GroupsForRemove, IsOptional = true)]
        public byte[] Remove;

        [DataMember(Code = (byte)ParameterKey.GroupsForAdd, IsOptional = true)]
        public byte[] Add;
    }
}