using System;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;

namespace Warsmiths.Server.Handlers.Craft
{
    public class SaveRecieptHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly RecieptRepository _recieptRepository = new RecieptRepository();

        public override OperationCode ControlCode => OperationCode.SaveReciept;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;
            var currentPlayer = peer.GetCurrentPlayer();

            var request = new SaveRecieptRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var reciept = request.RecieptJson.FromJson<BaseReciept>();

            if (reciept == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = $"can't desiarilize reciept"
                };
            }

            // new id
            if (string.IsNullOrEmpty(reciept._id))
            {
                reciept._id = Guid.NewGuid().ToString();
            }
           // if (!currentPlayer.SavedReciepts.Contains(reciept._id))
            {
              //  currentPlayer.SavedReciepts.Add(reciept._id);
            }

            peer.SendUpdatePlayerProfileEvent();

            var pr = new PlayerRepository();
            pr.Update(currentPlayer);
            
            _recieptRepository.Create(reciept);

            response = new OperationResponse(operationRequest.OperationCode,
                new CreateRecieptResponse { RecieptData = reciept.ToBson() })
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "saved"
            };

            return response;
        }
    }
}
