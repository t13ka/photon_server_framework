using Photon.SocketServer.Rpc;
using Warsmiths.Common;

namespace Warsmiths.Server.Operations.Response.Craft.Field.Common
{
    public class CreateElementResponse
    {
        [DataMember(Code = (byte)CraftCode.ElementID, IsOptional = false)]
        public byte[] RecieptData ;
    }
}