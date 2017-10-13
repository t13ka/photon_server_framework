using System;
using System.Collections.Generic;
using log4net;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Common
{
    public class PlayerCommon
    {
        public static void LevlUp(Player player, Character character, bool zerolevel = false)
        {
            character.CraftExperience -= (int)DomainConfiguration.CharacterCraftLevelsExperience[character.CraftLevel];

            if (!zerolevel)
            character.CraftLevel++;
          

            foreach (var a in DomainConfiguration.CraftLevelRewards[character.CraftLevel].LevelFeatures)
            {
                if (!player.LevelFeatures.Contains(a)) player.LevelFeatures.Add(a);
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
        }

        public static int GetFeatureLvl(LvlRewardFeaturesTypes feat)
        {
                foreach (var rew in DomainConfiguration.CraftLevelRewards)
                {
                    if (rew.LevelFeatures.Contains(feat))
                        return rew.Lvl;
                }
            return 0;
        }

        public static void DicreseCraftExpririence(Player player, Character character, int exp)
        {
            character.CraftExperience -= exp;
            if (character.CraftExperience <= 0)
            {
                while (DomainConfiguration.CharacterCraftLevelsExperience[character.CraftLevel] >
                       character.CraftExperience && character.CraftLevel >0)
                {
                    character.CraftLevel--;
                    character.CraftExperience = (int) (DomainConfiguration.CharacterCraftLevelsExperience[character.CraftLevel] + character.CraftExperience);
                }
            }
            var playerRepository = new PlayerRepository();
            character.Update();
            playerRepository.Update(player);
        }
        public static void GetCraftExpirience(Player player, Character character, int exp)
        {
            
            character.CraftExperience += exp;
            var playerRepository = new PlayerRepository();
            while (DomainConfiguration.CharacterCraftLevelsExperience[character.CraftLevel] <
                   character.CraftExperience && character.CraftLevel < DomainConfiguration.CharacterCraftLevelsExperience.Length-1)
            {
                LevlUp(player, character);
            }
            character.Update();
            playerRepository.Update(player);
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
