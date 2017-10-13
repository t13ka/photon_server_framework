using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class GetRecieptStageResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptStageData, IsOptional = false)]
        public byte[] RecieptStageData ;
    }
}
