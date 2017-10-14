using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Auction
{
    using YourGame.Common;

    public class UnpublishLotRequest : Operation
    {
        public UnpublishLotRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte) ParameterCode.LotId, IsOptional = false)]
        public string LotId ;
    }
}