using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class GetRecieptsResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptsData, IsOptional = false)]
        public byte[] RecieptData ;
    }
}