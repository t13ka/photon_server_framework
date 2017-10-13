using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateAuctionEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.AuctionData, IsOptional = false)]
        public byte[] AuctionData ;
    }
}
