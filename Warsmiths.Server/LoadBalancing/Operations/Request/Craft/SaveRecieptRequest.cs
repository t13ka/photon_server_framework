using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class SaveRecieptRequest : Operation
    {
        public SaveRecieptRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptJson, IsOptional = false)]
        public string RecieptJson ;
    }
}
