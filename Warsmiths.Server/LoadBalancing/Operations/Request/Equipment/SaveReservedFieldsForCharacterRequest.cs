using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Request.Equipment
{
    public class SaveReservedFieldsForCharacterRequest : Operation
    {
        public SaveReservedFieldsForCharacterRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// packed List<string> type
        /// </summary>
        [DataMember(Code = (byte)Warsmiths.Common.ParameterCode.RecieptsPage, IsOptional = true)]
        public byte[] Reserved1 ;


    }
}
