using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class CreateRecieptResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptData, IsOptional = false)]
        public byte[] RecieptData ;
    }
}