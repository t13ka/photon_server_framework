namespace YourGame.Common.Domain
{
    using System.Collections.Generic;

    using YourGame.Common.Domain.Enums;

    public class LevelingSystem
    {
        public static void LevlUp(Domain.Player player, Character character, bool zerolevel = false)
        {
            /*
            if (!zerolevel) character.CraftLevel++;
            character.CraftExperience = 0;

            foreach (var a in DomainConfiguration.CraftLevelRewards[character.CraftLevel].LevelFeatures)
            {
                if (!player.LevelFeatures.Contains(a))
                    player.LevelFeatures.Add(a);
            }

            foreach (var b in DomainConfiguration.CraftLevelRewards[character.CraftLevel].Reciepts)
            {
                if (character.Reciepts.Find(x => x == b) == null)
                {
                    var rec = MasterApplication.Reciepts.Find(x => x.Name == b);
                    character.Reciepts.Add(b);
                    rec._id = Guid.NewGuid().ToString();
                    rec.OwnerId = player._id;
                    new RecieptRepository().Create(rec);
                }
            }

            LogManager.GetLogger("").Debug("Lvl Up " + character.CraftLevel);
            */
        }

        public static int GetFeatureLvl(LvlRewardFeaturesTypes feat)
        {
            /*
                           foreach (var rew in DomainConfiguration.CraftLevelRewards)
                           {
                               if (rew.LevelFeatures.Contains(feat))
                                   return rew.Lvl;
                           }
           
                       return 0;
                   }
           
                   public void GetCraftExpirience(Player player, Character character, int exp)
                   {
                       character.CraftExperience += exp;
           
                       while (DomainConfiguration.CharacterCraftLevelsExperience[character.CraftLevel] <
                              character.CraftExperience && character.CraftLevel < DomainConfiguration.CharacterCraftLevelsExperience.Length)
                       {
           
                           LevlUp(player, character);
                       }
           
                       character.Update();
                       _playerRepository.Update(currentPlayer);
           
                       peer.SendUpdatePlayerProfileEvent();
                   }
                  */
            return 0;
        }
        public class LevelReward
        {
            public int Lvl;
            public List<string> Items;
            public List<string> Reciepts;
            public List<string> Tasks;
            public List<LvlRewardFeaturesTypes> LevelFeatures;
            public LevelReward()
            {
                Items = new List<string>();
                Reciepts = new List<string>();
                LevelFeatures = new List<LvlRewardFeaturesTypes>();
            }
        }
    }
}