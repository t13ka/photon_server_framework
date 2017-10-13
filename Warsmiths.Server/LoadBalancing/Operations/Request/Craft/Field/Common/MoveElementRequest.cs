using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Warsmiths.Common;

namespace Warsmiths.Server.Operations.Request.Craft.Field.Common
{
    public class MoveElementRequest : Operation
    {
        public MoveElementRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)CraftCode.ElementID, IsOptional = false)]
        public string Element ;

        [DataMember(Code = (byte)CraftCode.ElementPosition, IsOptional = false)]
        public byte[] ElementPosition;
    }
}