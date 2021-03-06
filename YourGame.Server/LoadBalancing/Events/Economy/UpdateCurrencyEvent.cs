﻿using Photon.SocketServer.Rpc;
using YourGame.Server.Operations;

namespace YourGame.Server.Events.Economy
{
    using YourGame.Common;

    public class UpdateCurrencyEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.CurrencyGold)]
        public int Gold;

        [DataMember(Code = (byte)ParameterCode.CurrencyCrystals)]
        public int Crystal;

        [DataMember(Code = (byte)ParameterCode.CurrencyKeys)]
        public int Keys;

        [DataMember(Code = (byte)ParameterCode.CurrenctyHealBox)]
        public int HealBox;
    }
}