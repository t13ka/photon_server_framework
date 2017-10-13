using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;

namespace Warsmiths.Server.Handlers.Craft
{
    public class GetReceptInfoHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly RecieptRepository _recieptRepository = new RecieptRepository();

        public override OperationCode ControlCode => OperationCode.GetRecieptInfo;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new GetRecieptRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var reciept = _recieptRepository.GetById(request.RecieptId);

            if (reciept == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                           {
                               ReturnCode =
                                   (short)ErrorCode.OperationFailed,
                               DebugMessage = $"can't find by id"
                           };
            }

            var qrec = reciept as BaseQuest;
            if (qrec != null)
            {
                qrec.StageList = null;
            }

            response = new OperationResponse(
                           operationRequest.OperationCode,
                           new GetRecieptResponse { RecieptData = reciept.ToBson() })
                           {
                               ReturnCode =
                                   (short)ErrorCode.Ok,
                               DebugMessage = $"ok"
                           };

            return response;
        }
    }
}