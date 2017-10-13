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
    public class GetRecieptStageHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly RecieptRepository _recieptRepository = new RecieptRepository();

        public override OperationCode ControlCode => OperationCode.GetRecieptStage;

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

            var reciept = _recieptRepository.GetById(request.RecieptId) as BaseQuest;

            if (reciept == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                           {
                               ReturnCode =
                                   (short)ErrorCode.OperationFailed,
                               DebugMessage = $"its not quest"
                           };
            }
            else
            {
                if (reciept.StageList.Count >= request.Stage)
                {
                    return new OperationResponse(operationRequest.OperationCode)
                               {
                                   ReturnCode =
                                       (short)ErrorCode
                                           .OperationFailed,
                                   DebugMessage = $"stage is wrong"
                               };
                }
            }

            response = new OperationResponse(
                           operationRequest.OperationCode,
                           new GetRecieptResponse { RecieptData = reciept.StageList[request.Stage].ToBson() })
                           {
                               ReturnCode
                                   =
                                   (
                                       short
                                   )
                                   ErrorCode
                                       .Ok,
                               DebugMessage
                                   =
                                   $"ok"
                           };

            return response;
        }
    }
}