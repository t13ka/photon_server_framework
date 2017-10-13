using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Server.Operations.Request.Tasks
{
    public class TaskCompleteRequest : Operation
    {
        public TaskCompleteRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.TaskType, IsOptional = false)]
        public TaskTypesE TaskType;

        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.TaskName, IsOptional = false)]
        public string TaskName;

       
    }
}
