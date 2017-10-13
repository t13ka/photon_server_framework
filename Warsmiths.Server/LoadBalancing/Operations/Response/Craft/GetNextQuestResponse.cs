using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class GetNextQuestResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.NextCraftQuest, IsOptional = false)]
        public byte[] RecieptData ;
    }
}