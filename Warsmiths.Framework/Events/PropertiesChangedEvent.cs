namespace YourGame.Server.Framework.Events
{
    using System.Collections;

    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

    public class PropertiesChangedEvent : RoomEventBase
    {
        public PropertiesChangedEvent(int actorNumber)
            : base(actorNumber)
        {
            Code = (byte)EventCode.PropertiesChanged;
        }

        [DataMember(Code = (byte)ParameterKey.Properties)]
        public Hashtable Properties;

        [DataMember(Code = (byte)ParameterKey.TargetActorNr)]
        public int TargetActorNumber;
    }
}