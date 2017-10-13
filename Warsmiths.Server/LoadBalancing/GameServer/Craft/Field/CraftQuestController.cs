using System;
using System.Collections.Generic;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Craft.SharedClasses;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Server.GameServer.Craft.Field
{
    public class CraftQuestController : CraftController
    {
        public int Stage;
        public int RecivedDamage=0;

        public CraftQuestController(BaseReciept rec, Player owner) : base(rec, owner)
        {
            Reciept = (BaseQuest)rec;
            CraftOwner = owner;

        }

        public override CraftResoult End()
        {
            Stats[StatTypes.CraftStar] = (int) ((Reciept.Steps - 2 * RecivedDamage) / 3f);
            Stats[StatTypes.Price] = Reciept.Status == RecieptStatus.IsFinish
                ? 0
                : Warsmiths.Common.Domain.Craft.Common.CalculateQuestExpirience(Stats[StatTypes.CraftStar],
                    Reciept.RequiredLevel, Stage);

            _log.Debug(Stats[StatTypes.Price]);
            Reciept.Status = RecieptStatus.IsFinish;

            var prize = ((BaseQuest) Reciept).RewardItems[0];

            if (prize == "Larec" || prize == "")
            {
                var prizeNumb = new Random((int) DateTime.Now.Ticks).Next(0, 3);
                var prizesTemp = new List<string>
                {
                    "Crystals",
                    "Keys",
                    "Coins"
                };
                prize = prizesTemp[prizeNumb];
            }
            ((BaseQuest) Reciept).RewardItems[0] = prize;

            Resoult = new CraftResoult
            {
                Expereince = Stats[StatTypes.Price],
                Stars = Stats[StatTypes.CraftStar],
                Prize = prize
            };
            return Resoult;
        }
    }
}
