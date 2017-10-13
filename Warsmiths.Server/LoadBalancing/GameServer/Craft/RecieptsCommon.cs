using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.GameServer.Craft
{
    public class RecieptsCommon
    {
        public static BaseQuest GetNextQuest(Player currentPlayer, string qName)
        {
            var currentCharacter = currentPlayer.GetCurrentCharacter();
            BaseReciept nextQuest;
            if (!string.IsNullOrEmpty(qName))
            {
                var currentQuest = MasterApplication.Reciepts.Find(x => x.Name == qName);
                if (currentQuest == null)
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(((BaseQuest)currentQuest).NextQuest))
                {
                    nextQuest = MasterApplication.Reciepts.Find(x => x.Name == ((BaseQuest)currentQuest).NextQuest) as BaseQuest;
                }
                else
                {
                    var qlvl = MasterApplication.GetQuestLvl(qName);
                    var nextQuestForLevel = "";
                    if (qlvl <= currentCharacter.CraftLevel) // && 
                    {
                        if (qlvl + 1 <= currentCharacter.CraftLevel)
                        {
                            nextQuestForLevel = qlvl + 1 < MasterApplication.QuestQuene.Count ? MasterApplication.QuestQuene[qlvl + 1] : "";
                        }
                        else
                        {
                            nextQuestForLevel = MasterApplication.GetNextQuest(qName);
                        }
                        if (nextQuestForLevel == qName)
                        {
                            if (qlvl + 1 < MasterApplication.QuestQuene.Count)
                            {
                                if (MasterApplication.GetQuestLvl(MasterApplication.QuestQuene[qlvl + 1]) <= currentCharacter.CraftLevel)
                                {
                                    nextQuest = MasterApplication.Reciepts.Find(x => x.Name == MasterApplication.QuestQuene[qlvl + 1]);
                                }
                                else
                                {
                                    return null;
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

                    }
                    else
                    {
                        var questQuene = Warsmiths.Common.Domain.Craft.Common.CalculateQuestQuene((BaseQuest)currentQuest, MasterApplication.Reciepts);
                        qName = questQuene[0].Name;

                        var questPosition = MasterApplication.QuestQuene.IndexOf(qName);

                        if (questPosition != -1)
                        {
                            var nextQuestPos = "";

                            nextQuestPos = MasterApplication.QuestQuene[questPosition + 1];
                            nextQuest = MasterApplication.Reciepts.Find(x => x.Name == nextQuestPos);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            else
            {
               return  null;
            }
            return (BaseQuest)nextQuest;
        }
    }
}
