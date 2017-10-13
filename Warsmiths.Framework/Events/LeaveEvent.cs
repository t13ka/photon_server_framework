using Photon.SocketServer.Rpc;

using Warsmiths.Server.Framework.Operations;

namespace Warsmiths.Server.Framework.Events
{
    public class LeaveEvent : RoomEventBase
    {
        public LeaveEvent(int actorNr, int[] actors)
            : base(actorNr)
        {
            Code = (byte)EventCode.Leave;
            Actors = actors;
        }

        [DataMember(Code = (byte)ParameterKey.Actors)]
        public int[] Actors;
    }
}