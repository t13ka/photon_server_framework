using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Auction
{
    using YourGame.Common;

    public class AuctionResponse
    {
        [DataMember(Code = (byte)ParameterCode.LotId, IsOptional = false)]
        public string LotId ;
    }
}
