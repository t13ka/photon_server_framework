using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class GetRecieptsRequest : Operation
    {
        public GetRecieptsRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptsPage, IsOptional = false)]
        public int Page ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptType, IsOptional = false)]
        public int Type ;
    }
}