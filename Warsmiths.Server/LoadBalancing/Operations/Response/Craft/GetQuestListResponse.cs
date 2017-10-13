using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class GetQuestListResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.GetAllReciepts, IsOptional = false)]
        public byte[] RecieptData ;
    }
}