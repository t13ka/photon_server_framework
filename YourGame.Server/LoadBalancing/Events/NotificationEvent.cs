﻿using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events
{
    using YourGame.Common;

    public class NotificationEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.NotificationData, IsOptional = false)]
        public byte[] NotificationData ;
    }
}
