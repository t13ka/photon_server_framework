using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Auction
{
    public class BuyLotRequest : Operation
    {
        public BuyLotRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.EquipmentId, IsOptional = false)]
        public string EquipmentId ;
    }
}