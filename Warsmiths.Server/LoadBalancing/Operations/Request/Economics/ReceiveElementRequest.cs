using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Economics
{
    public class ReceiveElementRequest : Operation
    {
        public ReceiveElementRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ElementId, IsOptional = false)]
        public string ElementId ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Quantity, IsOptional = false)]
        public int Quantity ;
    }
}
