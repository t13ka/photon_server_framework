using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Request.Auction
{
    using YourGame.Common;

    public class PublishLotRequest : Operation
    {
        public PublishLotRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
       
        [DataMember(Code = (byte) ParameterCode.EquipmentId, IsOptional = false)]
        public string EquipmentId ;

        [DataMember(Code = (byte) ParameterCode.Money, IsOptional = false)]
        public int Money ;
    }
}