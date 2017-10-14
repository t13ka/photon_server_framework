namespace YourGame.Server.Framework.Events
{
    using System;

    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

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