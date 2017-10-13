using System;
using Photon.SocketServer.Rpc;
using Warsmiths.Server.Framework.Operations;

namespace Warsmiths.Server.Framework.Events
{
    [Serializable]
    public abstract class RoomEventBase
    {
        protected RoomEventBase(int actorNr)
        {
            ActorNr = actorNr;
        }

        [DataMember(Code = (byte) ParameterKey.ActorNr)]
        public int ActorNr ;

        public byte Code ;
    }
}