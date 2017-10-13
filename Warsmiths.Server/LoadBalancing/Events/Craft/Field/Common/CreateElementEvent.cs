using System.Collections;
using Photon.SocketServer.Rpc;
using Warsmiths.Common;

namespace Warsmiths.Server.Events.Craft.Field.Common
{
    public class CreateElementEvent : DataContract
    {
        [DataMember(Code = (byte)CraftCode.ElementID)]
        public byte[] Element;
    }
}