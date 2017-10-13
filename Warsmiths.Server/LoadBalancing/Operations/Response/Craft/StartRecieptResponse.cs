using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class StartRecieptResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.StartReciept, IsOptional = false)]
        public byte[] RecieptData ;
    }
}