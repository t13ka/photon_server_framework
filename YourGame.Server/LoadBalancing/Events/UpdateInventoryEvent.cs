﻿using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events
{
    using YourGame.Common;

    public class UpdateInventoryEvent : DataContract
    {
        /// <summary>
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.InventoryData, IsOptional = false)]
        public byte[] InventoryData ;
    }
}
