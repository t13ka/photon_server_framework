using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events
{
    using YourGame.Common;

    public class UpdateAuctionEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.AuctionData, IsOptional = false)]
        public byte[] AuctionData ;
    }
}
