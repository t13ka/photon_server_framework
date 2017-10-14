namespace YourGame.Server.Framework.Events
{
    using System.Collections;

    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

    public class JoinEvent : RoomEventBase
    {
        public JoinEvent(int actorNr, int[] actors)
            : base(actorNr)
        {
            Code = (byte)EventCode.Join;
            Actors = actors;
        }

        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable ActorProperties;

        [DataMember(Code = (byte)ParameterKey.Actors)]
        public int[] Actors;
    }
}