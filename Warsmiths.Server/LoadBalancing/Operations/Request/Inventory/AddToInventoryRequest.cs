using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Inventory
{
    public class AddToInventoryRequest : Operation
    {
        public AddToInventoryRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Entity, IsOptional = false)]
        public string Entity ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Quantity, IsOptional = true)]
        public int Quantity ;
    }
}
