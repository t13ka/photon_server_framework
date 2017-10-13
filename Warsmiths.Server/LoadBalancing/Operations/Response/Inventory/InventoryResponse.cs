using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Inventory
{
    public class InventoryResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EntityId, IsOptional = false)]
        public string EntityId ;
    }
}
