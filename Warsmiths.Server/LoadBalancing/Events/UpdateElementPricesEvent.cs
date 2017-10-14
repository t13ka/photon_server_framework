using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    using YourGame.Common;

    public class UpdateElementPricesEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ElementsPrices, IsOptional = false)]
        public byte[] ElementPricedData ;
    }
}
