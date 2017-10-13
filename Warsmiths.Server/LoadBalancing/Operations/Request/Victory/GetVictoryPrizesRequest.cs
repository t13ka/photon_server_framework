using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Victory
{
    public class GetVictoryPrizesRequest : Operation
    {
        public GetVictoryPrizesRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /*
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.VictoryMastery)]
        public int Mastery ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.VictoryLuck)]
        public int Luck ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.VictoryMutualAid)]
        public int MutualAid ;*/
    }
}
