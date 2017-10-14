using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Response
{
    public class DebugGameResponse
    {

        [DataMember(Code = (byte)ParameterCode.Address, IsOptional = true)]
        public string Address ;

        [DataMember(Code = (byte)ParameterCode.NodeId, IsOptional = true)]
        public byte NodeId ;

        [DataMember(Code = (byte)ParameterCode.Info)]
        public string Info ;
    }
}
