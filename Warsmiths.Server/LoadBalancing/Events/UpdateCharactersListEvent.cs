using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    using YourGame.Common;

    public class UpdateCharactersListEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.CharactersListData, IsOptional = false)]
        public byte[] CharactersListData ;
    }
}
