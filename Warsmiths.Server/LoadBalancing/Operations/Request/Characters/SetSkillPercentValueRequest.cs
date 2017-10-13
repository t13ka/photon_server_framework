using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Characters
{
    public class SetSkillPercentValueRequest : Operation
    {
        public SetSkillPercentValueRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.SkillPercent, IsOptional = false)]
        public int SkillPercent ;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CharacteristicType, IsOptional = false)]
        public int CharacteristicType ;
    }
}