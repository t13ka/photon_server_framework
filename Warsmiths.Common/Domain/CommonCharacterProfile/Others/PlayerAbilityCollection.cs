using System.Collections.Generic;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Others
{
    public class PlayerAbilityCollection
    {
        private static Dictionary<string, PlayerClassAbility> _abilities;

        public static Dictionary<string, PlayerClassAbility> Abilities
        {
            get
            {
                if (_abilities == null) GenereateData();
                return _abilities;
            }
        }

        private static void GenereateData()
        {
            _abilities = new Dictionary<string, PlayerClassAbility>();
            var classes = PlayerClassCollection.Classes;
            var x = 1;
            foreach (var gameClass in classes)
            {
                if (gameClass.Key == ClassTypes.None) continue;

                gameClass.Value.Abilities = new PlayerClassAbility[6];
                for (var i = 0; i < 6; i++)
                {
                    gameClass.Value.Abilities[i] = new PlayerClassAbility
                    {
                        Name = x.ToString(),
                        Type = (AbilityTypeE)(i / 2 % 3),
                        Class = gameClass.Key,
                        Desc = "Description of " + x,
                    };
                    x++;
                    _abilities.Add(gameClass.Value.Abilities[i].Name, gameClass.Value.Abilities[i]);
                }
            }
        }
    }
}
