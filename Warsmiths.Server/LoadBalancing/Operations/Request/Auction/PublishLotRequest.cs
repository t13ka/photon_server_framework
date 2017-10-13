using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Auction
{
    public class PublishLotRequest : Operation
    {
        public PublishLotRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
       
        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.EquipmentId, IsOptional = false)]
        public string EquipmentId ;

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.Money, IsOptional = false)]
        public int Money ;
    }
}