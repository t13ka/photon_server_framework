using Photon.SocketServer.Rpc;
using Warsmiths.Common;

namespace Warsmiths.Server.Events.Craft.Field.Common
{
    public class MoveElementEvent : DataContract
    {
        [DataMember(Code = (byte)CraftCode.ElementPosition)]
        public  byte[,] Data ;

        [DataMember(Code = (byte)CraftCode.ElementID)]
        public byte[] Element ;
    }
}