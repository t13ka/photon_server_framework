using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class GetRecieptInfoResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptInfo, IsOptional = false)]
        public byte[] RecieptData ;
    }
}
