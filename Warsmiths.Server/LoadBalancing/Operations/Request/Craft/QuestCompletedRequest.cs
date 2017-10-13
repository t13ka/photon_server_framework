using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class QuestCompletedRequest : Operation
    {
        public QuestCompletedRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CraftQuestComplete, IsOptional = false)]
        public string RecieptName;
    }
}
