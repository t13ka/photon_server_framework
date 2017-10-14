namespace YourGame.Server.Framework.Events
{
    using System;

    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

    [Serializable]
    public abstract class RoomEventBase
    {
        protected RoomEventBase(int actorNr)
        {
            ActorNr = actorNr;
        }

        [DataMember(Code = (byte)ParameterKey.ActorNr)]
        public int ActorNr;

        public byte Code;
    }
}