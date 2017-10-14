namespace YourGame.Common.Domain.Craft.Quest
{
    using System.Collections.Generic;

    using YourGame.Common.Domain.VictoryPrizes;

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
