using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Craft
{
    public class SetCraftExperienceRequest : Operation
    {
        public SetCraftExperienceRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CraftExperience, IsOptional = false)]
        public int CraftExperience ;
    }
}