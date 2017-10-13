using System;
using System.Collections.Generic;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Craft.SharedClasses;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Common;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.GameServer.Craft;
using Warsmiths.Server.GameServer.Craft.Field;
using Warsmiths.Server.GameServer.Tasks.Common;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;
using CraftQuestController = Warsmiths.Server.GameServer.Craft.Field.CraftQuestController;

namespace Warsmiths.Server.Handlers.Craft
{
    public class EndRecieptHandler : BaseHandler
    {

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

       // private readonly RecieptRepository _recieptRepository = new RecieptRepository();
        private readonly PlayerRepository _playerRepository = new PlayerRepository();
        private readonly RecieptRepository _recieptRepository = new RecieptRepository();

        public override OperationCode ControlCode => OperationCode.EndReciept;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            var peer = (MasterClientPeer)peerBase;
            var currentPlayer = peer.GetCurrentPlayer();
            if (!MasterApplication.CraftFields.ContainsKey(currentPlayer.Login))
            {
                return new OperationResponse { ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = "No active quest" };
            }
 
            var request = new EndRecieptRequest(peer.Protocol, operationRequest);

            var currentCharacter = currentPlayer.GetCurrentCharacter();

            CraftResoult resoult;
            var endQuest = request.Reciept.FromBson<BaseReciept>();

            var qrec = endQuest as BaseQuest;
            if (qrec != null)
            {
                var qName = endQuest.Name;

                var qrecField = MasterApplication.CraftFields[currentPlayer.Login] as CraftQuestController;

                if (qrecField != null)
                {
                    qrecField.Reciept = (BaseQuest)endQuest;
                    qrecField.RecivedDamage = request.RecivedDamage;
                    qrecField.Stage = request.Stage;
                    resoult = qrecField.End();

                    _log.Debug(" Stage  " + request.Stage);
                    _log.Debug(" RecivedDamage  " + request.RecivedDamage);
                    _log.Debug(" resoult reward  " + resoult.Prize);
                    _log.Debug(" resoult reward  " + ((BaseQuest)qrecField.Reciept).RewardItems[0]);
                    _log.Debug(" exp reward  " + resoult.Expereince);

                    PlayerCommon.GetCraftExpirience(currentPlayer, currentCharacter, resoult.Expereince);
                }
                else
                {
                    _log.Debug("Error");
                    return new OperationResponse
                    {
                        ReturnCode = (short)ErrorCode.OperationInvalid,
                        DebugMessage = "Somethink went wrong"
                    };
                }
                _log.Debug(currentCharacter.CompletedQuests + " Quest Compelete " + ":" + endQuest.Name);
                if (!currentCharacter.CompletedQuests.Contains(qName)) currentCharacter.CompletedQuests.Add(qName);
                var nextQuest = RecieptsCommon.GetNextQuest(currentPlayer, qName);

                if (nextQuest != null)
                {
                    _log.Debug(qName + "Next quest is " + nextQuest.Name);

                    if (currentCharacter.Reciepts.Find(x => x == nextQuest.Name) == null)
                    {
                        _log.Debug("Create next recept in recpository" + nextQuest.StageList.Count);
                        currentCharacter.Reciepts.Add(nextQuest.Name);
                        nextQuest._id = Guid.NewGuid().ToString();
                        nextQuest.OwnerId = currentPlayer._id;
                        _recieptRepository.Create(nextQuest);
                    }
                }
                else
                {
                    resoult.WarningMessage = "Not enought exp for next quest";
                }

                 _recieptRepository.Replace(qrecField.Reciept);
                _recieptRepository.Update(qrecField.Reciept);

            
            } else if (endQuest != null)
            {
                resoult = MasterApplication.CraftFields[currentPlayer.Login].End();
                _log.Debug("Custom Reciept Ended");
            }
            else
            {
                _log.Debug("Quest is already ended");
                return new OperationResponse { ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = "No quest exist" };
            }

            _log.Debug("Check task");
            var task = TaskOperations.CheckTask(currentPlayer, currentCharacter);
            if (task != null && task.Status ==  TaskStatusTypesE.Finished)
            {
                _log.Debug("TaskCompeteed");
            }


             currentCharacter.Update();
            _playerRepository.Update(currentPlayer);
            peer.SendUpdatePlayerProfileEvent();

            var response = new OperationResponse(operationRequest.OperationCode,
                new EndRecieptResponse { Resoult = resoult.ToBson()})
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            return response;
        }
    }
}
