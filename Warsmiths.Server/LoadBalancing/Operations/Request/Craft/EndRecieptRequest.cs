using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class EndRecieptRequest : Operation
    {
        public EndRecieptRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptData, IsOptional = false)]
        public byte[] Reciept ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecivedDamage, IsOptional = false)]
        public int RecivedDamage;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Stage, IsOptional = false)]
        public int Stage;
    }
}