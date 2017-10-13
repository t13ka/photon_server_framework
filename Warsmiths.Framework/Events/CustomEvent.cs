using System;

using Photon.SocketServer.Rpc;

using Warsmiths.Server.Framework.Operations;

namespace Warsmiths.Server.Framework.Events
{
    [Serializable]
    public class CustomEvent : RoomEventBase
    {
        public CustomEvent(int actorNr, byte eventCode, object data)
            : base(actorNr)
        {
            Code = eventCode;
            Data = data;
        }

        [DataMember(Code = (byte)ParameterKey.Data, IsOptional = true)]
        public object Data;
    }
}