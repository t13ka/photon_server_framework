using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Economics
{
    public class ChangeCurrencyRequest : Operation
    {
        public ChangeCurrencyRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CurrencyType, IsOptional = false)]
        public int CurrencyType;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CurrencyValue, IsOptional = false)]
        public int CurrencyValue;
    }
}