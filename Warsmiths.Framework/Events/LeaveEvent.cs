namespace YourGame.Server.Framework.Events
{
    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

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