using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetNextQuestHandler : BaseHandler
    {

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public override OperationCode ControlCode => OperationCode.CraftQuestNext;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            _log.Debug("get next quest");
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;
          //  var request = new GetNextQuestRequest(peer.Protocol, operationRequest);

            var qlist = new List<string>();
            var currentPlayer = peer.GetCurrentPlayer();
            var character = currentPlayer.GetCurrentCharacter();
            BaseReciept nextQuest;

            var qName = "";
            if (character.CompletedQuests.Count > 0)
            {
                qName = character.CompletedQuests[character.CompletedQuests.Count - 1];
            }

            if (!string.IsNullOrEmpty(qName))
            {
                var currentQuest = MasterApplication.Reciepts.Find(x => x.Name == qName);
                if (currentQuest == null)
                {
                    _log.Debug(qName + " no found ");
                    return new OperationResponse();
                }

                if(!string.IsNullOrEmpty( ((BaseQuest)currentQuest).NextQuest) )
                {
                    nextQuest = MasterApplication.Reciepts.Find(x => x.Name == ((BaseQuest)currentQuest).NextQuest) as BaseQuest;
                    _log.Debug("Is next sub quest" + nextQuest.Name);
                }
                else
                {
                    _log.Debug("Is next new quest");
                    var qlvl = MasterApplication.GetQuestLvl(qName);
                    _log.Debug("Current expirience is"  + currentPlayer.Characters[0].CraftLevel);
                    _log.Debug("Current quest level is " + qlvl);

                    var nextQuestForLevel = "";
                    if (qlvl <= currentPlayer.Characters[0].CraftLevel) // && 
                    {
                        if (qlvl + 1 <= currentPlayer.Characters[0].CraftLevel)
                        {
                            _log.Debug("Current Craft lvl is enought for next lvl" + qlvl);
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
                        _log.Debug(qName +" next is " + nextQuestForLevel);
                        if (nextQuestForLevel == qName)
                        {
                            _log.Debug(" isLastQuest  ");
                            if (qlvl + 1 < MasterApplication.QuestQuene.Count)
                            {
                                _log.Debug(" Maybe is  " + MasterApplication.QuestQuene[qlvl + 1] + " :lvl " + MasterApplication.QuestQuene[qlvl + 1]);
                                if (MasterApplication.GetQuestLvl(MasterApplication.QuestQuene[qlvl + 1]) <= currentPlayer.Characters[0].CraftLevel)
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
                       _log.Debug(" no found " + qName);
                       nextQuest = MasterApplication.Reciepts.Find(x => x.Name == "Quest0_1");
                    }
                }
            }
            else
            {
                nextQuest = MasterApplication.Reciepts.Find(x => x.Name == "Quest0_1");
                _log.Debug(" empty name go default Quest0_1 ");
            }

            _log.Debug(" Send next Quest " + nextQuest.Name);
            response = new OperationResponse(operationRequest.OperationCode,
                new GetNextQuestResponse { RecieptData = nextQuest.ToBson()  })
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            return response;
        }
    }
}
