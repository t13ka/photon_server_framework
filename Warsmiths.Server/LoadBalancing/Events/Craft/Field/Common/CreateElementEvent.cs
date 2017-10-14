using System.Collections;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events.Craft.Field.Common
{
    using YourGame.Common;

    public class CreateElementEvent : DataContract
    {
        [DataMember(Code = (byte)CraftCode.ElementID)]
        public byte[] Element;
    }
}