using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class QuestCompleteResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CraftQuestComplete, IsOptional = false)]
        public byte[] RecieptName ;
    }
}