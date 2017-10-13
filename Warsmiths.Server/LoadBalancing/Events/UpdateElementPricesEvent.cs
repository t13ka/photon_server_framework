using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateElementPricesEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.ElementsPrices, IsOptional = false)]
        public byte[] ElementPricedData ;
    }
}
