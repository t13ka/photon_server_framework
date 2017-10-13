using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Equipment
{
    public class CreateArmorRequest : Operation
    {
        public CreateArmorRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptData, IsOptional = false)]
        public byte[] RecieptData ;
    }
}