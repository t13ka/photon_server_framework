using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class VictoryPrizesEvent: DataContract
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.VictoryPrizes, IsOptional = false)]
        public byte[] Prizes ;
    }
}
