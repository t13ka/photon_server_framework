using Photon.SocketServer.Rpc;
using Warsmiths.Common.Domain.Craft.SharedClasses;

namespace Warsmiths.Server.Operations.Response.Craft
{
    public class EndRecieptResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.EndReciept, IsOptional = false)]
        public byte[] Resoult ;
    }
}