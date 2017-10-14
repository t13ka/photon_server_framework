using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Response
{
    public class JoinGameResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterCode.Address, IsOptional = false)]
        public string Address ;

        [DataMember(Code = (byte)ParameterCode.NodeId)]
        public byte NodeId ;

        #endregion
    }
}