using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Tasks
{
    public class TaskCompleteResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.TaskComplete, IsOptional = false)]
        public byte[] CompletedTask ;
    }
}