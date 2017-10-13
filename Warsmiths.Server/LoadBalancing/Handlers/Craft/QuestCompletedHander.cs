using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;

namespace Warsmiths.Server.Handlers.Craft
{
    public class QuestCompletedHander : BaseHandler
    {
        public class QuestResoult
        {
            public int Stars;
        }

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.CraftQuestComplete;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;
            var request = new QuestCompletedRequest(peer.Protocol, operationRequest);

            var currentPlayer = peer.GetCurrentPlayer();
            var character = currentPlayer.GetCurrentCharacter();

            /*_log.Debug(currentPlayer.CraftQuestCompleted + " Quest Compelete " + ":" + request.RecieptName);
            var qName = request.RecieptName;
            if(!currentPlayer.CraftQuestCompleted.Contains(qName)) currentPlayer.CraftQuestCompleted.Add(qName);

            BaseReciept nextQuest;
            if (!string.IsNullOrEmpty(qName))
            {
                var currentQuest = MasterApplication.Reciepts.Find(x => x.Name == qName);
                if (currentQuest == null)
                {
                    _log.Debug(qName + " no found ");
                    return new OperationResponse();
                }

                if (!string.IsNullOrEmpty(((BaseQuest)currentQuest).NextQuest))
                {
                    nextQuest = MasterApplication.Reciepts.Find(x => x._id == ((BaseQuest)currentQuest).NextQuest) as BaseQuest;
                    _log.Debug("Is next sub quest" + nextQuest.Name);
                }
                else
                {
                    _log.Debug("Is next new quest");
                    var qlvl = MasterApplication.GetQuestLvl(qName);
                    _log.Debug("Current expirience is" + currentPlayer.Characters[0].CraftLevel);
                    _log.Debug("Current quest level is " + qlvl);
                    var nextQuestForLevel = "";
                    if (qlvl <= currentPlayer.Characters[0].CraftLevel) // && 
                    {
                        if (qlvl + 1 <= currentPlayer.Characters[0].CraftLevel)
                        {
                            _log.Debug("Current Craft lvl is enought for next lvl, is " + qlvl + 1);
                            if (qlvl + 1 < MasterApplication.QuestQuene.Count)
                            {
                                nextQuestForLevel = MasterApplication.QuestQuene[qlvl + 1];
                            }
                            else
                            {
                                _log.Debug("End, no quest..");
                                nextQuestForLevel = "Quest0_1";
                            }

                        }
                        else
                        {
                            nextQuestForLevel = MasterApplication.GetNextQuest(qName);
                        }
                        _log.Debug(qName + " next is " + nextQuestForLevel);
                        if (nextQuestForLevel == qName)
                        {
                            _log.Debug(" isLastQuest  ");
                            if (qlvl + 1 < MasterApplication.QuestQuene.Count)
                            {
                                _log.Debug(" Maybe is  " + MasterApplication.QuestQuene[qlvl + 1] +" :lvl " + MasterApplication.QuestQuene[qlvl + 1]);
                                if(MasterApplication.GetQuestLvl(MasterApplication.QuestQuene[qlvl + 1]) <= currentPlayer.Characters[0].CraftLevel)
                                {
                                    nextQuest = MasterApplication.Reciepts.Find(x => x.Name == MasterApplication.QuestQuene[qlvl + 1]);
                                }
                                else
                                {
                                    _log.Debug(" No lvlv  ");
                                    nextQuest = MasterApplication.Reciepts.Find(x => x.Name == "Quest0_1");
                                }
                      
                            }
                            else
                            {
                                nextQuest = MasterApplication.Reciepts.Find(x => x.Name == nextQuestForLevel);
                            }
                        }
                        else
                        {
                            nextQuest = MasterApplication.Reciepts.Find(x => x.Name == nextQuestForLevel);
                        }

                        _log.Debug(" next quest for lvl " + nextQuest.Name);
                    }
                    else
                    {
                        var questQuene = Warsmiths.Common.Domain.Craft.Common.CalculateQuestQuene((BaseQuest)currentQuest, MasterApplication.Reciepts);

                        qName = questQuene[0].Name;

                        var questPosition = MasterApplication.QuestQuene.IndexOf(qName);

                        if (questPosition != -1)
                        {
                            _log.Debug(questPosition + " position ");
                            var nextQuestPos = "";

                            nextQuestPos = MasterApplication.QuestQuene[questPosition + 1];
                            nextQuest = MasterApplication.Reciepts.Find(x => x.Name == nextQuestPos);
                            _log.Debug(nextQuest + " next quest ");
                        }
                        else
                        {
                            _log.Debug(" no found " + qName);
                            nextQuest = MasterApplication.Reciepts.Find(x => x.Name == "Quest0_1");
                        }
                    }
                }
            }
            else
            {
                nextQuest = MasterApplication.Reciepts.Find(x => x.Name == "Quest0_1");
                _log.Debug(" empty name go default Quest0_1 ");
            }
            if(currentPlayer.Reciepts.Find(x=>x.Name == nextQuest.Name) == null)
            {
                nextQuest.StageList = null;
                currentPlayer.Reciepts.Add(nextQuest);
            }

            _playerRepository.Update(currentPlayer);
            var pr = new PlayerRepository();
            pr.Update(currentPlayer);
            peer.SendUpdatePlayerProfileEvent();

            //if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
             //   return response;
            }
            */
            response = new OperationResponse(
                           operationRequest.OperationCode,
                           new QuestCompleteResponse { RecieptName = new BaseQuest().ToBson() })
                           {
                               ReturnCode =
                                   (short)
                                   ErrorCode.Ok,
                               DebugMessage = "ok"
                           };

            return response;
        }
    }
}