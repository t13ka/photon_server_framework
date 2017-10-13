using System;
using System.Collections.Generic;
using ExitGames.Logging;
using Newtonsoft.Json;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;

namespace Warsmiths.Server.Handlers.Craft
{
    public class GetQuestListHandler : BaseHandler
    {

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly RecieptRepository _recieptRepository = new RecieptRepository();
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.GetAllReciepts;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            _log.Debug("get all reciepts");
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;
            var currentPlayer = peer.GetCurrentPlayer();
            var character = currentPlayer.GetCurrentCharacter();

            var request = new GetQuestListRequest(peer.Protocol, operationRequest);
            //if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
             //   return response;
            }

            //GET ALL
            foreach (var a in DomainConfiguration.CraftLevelRewards)
            {
                foreach (var b in a.LevelFeatures)
                {
                    if (!currentPlayer.LevelFeatures.Contains(b))
                        currentPlayer.LevelFeatures.Add(b);
                }//3
            }
            foreach (var item in MasterApplication.Items)
            {
                if(item is BasePerk && currentPlayer.NoneInventoryItems.Find(x=>x.Name == item.Name) == null)
                currentPlayer.NoneInventoryItems.Add(item);
            }

            foreach (var rec in MasterApplication.Reciepts)
            {
                if (character.Reciepts.Find(x => x == rec.Name) == null)
                {
                    character.Reciepts.Add(rec.Name);
                    rec._id = Guid.NewGuid().ToString();
                    rec.OwnerId = currentPlayer._id;
                    new RecieptRepository().Create(rec);
                }
            }
            character.Update();
            _playerRepository.Update(currentPlayer);

            peer.SendUpdatePlayerProfileEvent();

            response = new OperationResponse(operationRequest.OperationCode,
                new GetQuestListResponse { RecieptData = new BaseReciept().ToBson()})
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            return response;
        }
    }
}
