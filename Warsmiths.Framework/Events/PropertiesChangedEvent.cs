using System.Collections;

using Photon.SocketServer.Rpc;

using Warsmiths.Server.Framework.Operations;

namespace Warsmiths.Server.Framework.Events
{
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