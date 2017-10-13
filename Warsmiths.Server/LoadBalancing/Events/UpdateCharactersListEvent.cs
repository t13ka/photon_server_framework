using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Events
{
    public class UpdateCharactersListEvent : DataContract
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.CharactersListData, IsOptional = false)]
        public byte[] CharactersListData ;
    }
}
