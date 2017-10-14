using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    using YourGame.Common;

    public class VictoryPrizesEvent: DataContract
    {
        [DataMember(Code = (byte)ParameterCode.VictoryPrizes, IsOptional = false)]
        public byte[] Prizes ;
    }
}
