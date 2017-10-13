using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Characters
{
    public class CreateCharacterRequest : Operation
    {

        public CreateCharacterRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.Name, IsOptional = false)]
        public string Name ;

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.Race, IsOptional = false)]
        public int Race ;

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.Hero, IsOptional = false)]
        public int Hero ;

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.StartBonus, IsOptional = false)]
        public int StartBonus ;

        [DataMember(Code = (byte) Warsmiths.Common.ParameterCode.CharacterClass, IsOptional = false)]
        public int ClassTypes ;
    }
}