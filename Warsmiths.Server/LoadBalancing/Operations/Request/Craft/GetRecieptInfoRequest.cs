using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class GetRecieptInfoRequest : Operation
    {
        public GetRecieptInfoRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptInfo, IsOptional = false)]
        public string RecieptId ;
}
}