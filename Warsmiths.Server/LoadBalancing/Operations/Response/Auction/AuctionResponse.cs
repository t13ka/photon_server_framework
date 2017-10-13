using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Auction
{
    public class AuctionResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.LotId, IsOptional = false)]
        public string LotId ;
    }
}
