using System.Collections.Generic;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.VictoryPrizes;

namespace Warsmiths.Common.Domain.Craft.Quest
{
    public class BaseQuest : BaseReciept
    {
        public List<string> QuestTyps ;
        public List<string> RewardItems;
        public List<VictoryPrize> Rewards ;
        public string NextQuest ;
        public string PrevQuest ;

        public RecieptBackground Background;
    }
}
