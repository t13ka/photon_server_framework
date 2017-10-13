using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Victory
{
    public class SelectVictoryPrizesRequest : Operation
    {
        public SelectVictoryPrizesRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.VictoryIds)]
        public int[] Ids ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Money)]
        public int Money ;
        
    }
}
