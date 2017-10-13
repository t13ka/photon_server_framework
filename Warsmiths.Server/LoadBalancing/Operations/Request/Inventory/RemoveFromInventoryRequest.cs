using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Inventory
{
    public class RemoveFromInventoryRequest : Operation
    {
        public RemoveFromInventoryRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.Entity, IsOptional = false)]
        public string Entity ;
    }
}