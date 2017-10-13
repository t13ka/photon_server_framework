using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response.Character
{
    public class CharacterCreatedResponse
    {
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.Name, IsOptional = false)]
        public string CharacterName ;
    }
}
