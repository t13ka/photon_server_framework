using Photon.SocketServer.Rpc;
using Warsmiths.Server.Operations;

namespace Warsmiths.Server.Events.Economy
{
    public class UpdateCurrencyEvent : DataContract
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CurrencyGold)]
        public int Gold;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CurrencyCrystals)]
        public int Crystal;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CurrencyKeys)]
        public int Keys;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CurrenctyHealBox)]
        public int HealBox;
    }
}