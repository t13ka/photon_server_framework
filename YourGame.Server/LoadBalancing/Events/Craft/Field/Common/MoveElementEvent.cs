using Photon.SocketServer.Rpc;

namespace YourGame.Server.Events.Craft.Field.Common
{
    using YourGame.Common;

    public class MoveElementEvent : DataContract
    {
        [DataMember(Code = (byte)CraftCode.ElementPosition)]
        public  byte[,] Data ;

        [DataMember(Code = (byte)CraftCode.ElementID)]
        public byte[] Element ;
    }
}