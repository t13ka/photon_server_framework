using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class StartRecieptRequest : Operation
    {
        public StartRecieptRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.StartReciept, IsOptional = false)]
        public byte[] Reciept ;
}
}