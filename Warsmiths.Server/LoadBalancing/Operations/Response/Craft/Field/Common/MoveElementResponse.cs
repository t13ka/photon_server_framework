using Photon.SocketServer.Rpc;
using Warsmiths.Common;

namespace Warsmiths.Server.Operations.Response.Craft.Field.Common
{
    public class MoveElementResponse
    {
        [DataMember(Code = (byte)CraftCode.ElementID, IsOptional = false)]
        public byte[] Element ;

        [DataMember(Code = (byte)CraftCode.ElementPosition, IsOptional = false)]
        public byte[] Position ;
    }
}