using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class GetNextQuestRequest : Operation
    {
        public GetNextQuestRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.NextCraftQuest, IsOptional = false)]
        public string ReceptName ;
}
}