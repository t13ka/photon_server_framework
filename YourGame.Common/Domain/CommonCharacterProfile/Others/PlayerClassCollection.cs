namespace YourGame.Common.Domain.CommonCharacterProfile.Others
{
    using System.Collections.Generic;

    using YourGame.Common.Domain.Enums;

    public static class PlayerClassCollection
    {
        private static Dictionary<ClassTypes, PlayerGameClass> _classes;

        public static Dictionary<ClassTypes, PlayerGameClass> Classes
        {
            get
            {
                if (_classes == null) GenereateData();
                return _classes;
            }
        }

        private static void GenereateData()
        {
            _classes = new Dictionary<ClassTypes, PlayerGameClass>
            {
                {
                    ClassTypes.None, null
                },
                {
                    ClassTypes.Warlock, new PlayerGameClass
                    {
                        Name = "Warlock",
                        Type = ClassTypes.Warlock,
                        Characteristic = CharacteristicE.Power,
                        Abilities = null,
                        PositionOnLine = 1,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Warlord, new PlayerGameClass
                    {
                        Name = "Warlord",
                        Type = ClassTypes.Warlord,
                        Characteristic = CharacteristicE.Speed,
                        Abilities = null,
                        PositionOnLine = 1,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Master, new PlayerGameClass
                    {
                        Name = "Master",
                        Type = ClassTypes.Master,
                        Characteristic = CharacteristicE.Intellect,
                        Abilities = null,
                        PositionOnLine = 1,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Guard, new PlayerGameClass
                    {
                        Name = "Guard",
                        Type = ClassTypes.Guard,
                        Characteristic = CharacteristicE.Endurance,
                        Abilities = null,
                        PositionOnLine = 1,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Chosen, new PlayerGameClass
                    {
                        Name = "Chosen",
                        Type = ClassTypes.Chosen,
                        Characteristic = CharacteristicE.Dexterity,
                        Abilities = null,
                        PositionOnLine = 1,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Hunter, new PlayerGameClass
                    {
                        Name = "Hunter",
                        Type = ClassTypes.Hunter,
                        Characteristic = CharacteristicE.Wisdom,
                        Abilities = null,
                        PositionOnLine = 1,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Shadow, new PlayerGameClass
                    {
                        Name = "Shadow",
                        Type = ClassTypes.Shadow,
                        Characteristic = CharacteristicE.Power,
                        Abilities = null,
                        PositionOnLine = 2,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Warrior, new PlayerGameClass
                    {
                        Name = "Warrior",
                        Type = ClassTypes.Warrior,
                        Characteristic = CharacteristicE.Speed,
                        Abilities = null,
                        PositionOnLine = 2,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Enchanter, new PlayerGameClass
                    {
                        Name = "Enchanter",
                        Type = ClassTypes.Enchanter,
                        Characteristic = CharacteristicE.Intellect,
                        Abilities = null,
                        PositionOnLine = 2,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Titan, new PlayerGameClass
                    {
                        Name = "Titan",
                        Type = ClassTypes.Titan,
                        Characteristic = CharacteristicE.Endurance,
                        Abilities = null,
                        PositionOnLine = 2,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Knight, new PlayerGameClass
                    {
                        Name = "Knight",
                        Type = ClassTypes.Knight,
                        Characteristic = CharacteristicE.Dexterity,
                        Abilities = null,
                        PositionOnLine = 2,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Alchemist, new PlayerGameClass
                    {
                        Name = "Alchemist",
                        Type = ClassTypes.Alchemist,
                        Characteristic = CharacteristicE.Wisdom,
                        Abilities = null,
                        PositionOnLine = 2,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Leader, new PlayerGameClass
                    {
                        Name = "Leader",
                        Type = ClassTypes.Leader,
                        Characteristic = CharacteristicE.Power,
                        Abilities = null,
                        PositionOnLine = 3,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Duelist, new PlayerGameClass
                    {
                        Name = "Duelist",
                        Type = ClassTypes.Duelist,
                        Characteristic = CharacteristicE.Speed,
                        Abilities = null,
                        PositionOnLine = 3,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Ranger, new PlayerGameClass
                    {
                        Name = "Ranger",
                        Type = ClassTypes.Ranger,
                        Characteristic = CharacteristicE.Intellect,
                        Abilities = null,
                        PositionOnLine = 3,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Immortal, new PlayerGameClass
                    {
                        Name = "Immortal",
                        Type = ClassTypes.Immortal,
                        Characteristic = CharacteristicE.Endurance,
                        Abilities = null,
                        PositionOnLine = 1,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Catcher, new PlayerGameClass
                    {
                        Name = "Catcher",
                        Type = ClassTypes.Catcher,
                        Characteristic = CharacteristicE.Dexterity,
                        Abilities = null,
                        PositionOnLine = 3,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Judge, new PlayerGameClass
                    {
                        Name = "Judge",
                        Type = ClassTypes.Judge,
                        Characteristic = CharacteristicE.Wisdom,
                        Abilities = null,
                        PositionOnLine = 3,
                        Sinergy = null,
                        Desc = "",
                        Tier = 1
                    }
                },
                {
                    ClassTypes.Mag, new PlayerGameClass
                    {
                        Name = "Mag",
                        Type = ClassTypes.Mag,
                        Characteristic = CharacteristicE.Power,
                        Abilities = null,
                        PositionOnLine = 4,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Foreseer, new PlayerGameClass
                    {
                        Name = "Foreseer",
                        Type = ClassTypes.Foreseer,
                        Characteristic = CharacteristicE.Speed,
                        Abilities = null,
                        PositionOnLine = 4,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Gravitor, new PlayerGameClass
                    {
                        Name = "Gravitor",
                        Type = ClassTypes.Gravitor,
                        Characteristic = CharacteristicE.Intellect,
                        Abilities = null,
                        PositionOnLine = 4,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Tunneler, new PlayerGameClass
                    {
                        Name = "Tunneler",
                        Type = ClassTypes.Tunneler,
                        Characteristic = CharacteristicE.Endurance,
                        Abilities = null,
                        PositionOnLine = 4,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Eferal, new PlayerGameClass
                    {
                        Name = "Eferal",
                        Type = ClassTypes.Eferal,
                        Characteristic = CharacteristicE.Dexterity,
                        Abilities = null,
                        PositionOnLine = 4,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Mentalist, new PlayerGameClass
                    {
                        Name = "Mentalist",
                        Type = ClassTypes.Mentalist,
                        Characteristic = CharacteristicE.Wisdom,
                        Abilities = null,
                        PositionOnLine = 4,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Construct, new PlayerGameClass
                    {
                        Name = "Construct",
                        Type = ClassTypes.Construct,
                        Characteristic = CharacteristicE.Power,
                        Abilities = null,
                        PositionOnLine = 5,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Teleport, new PlayerGameClass
                    {
                        Name = "Teleport",
                        Type = ClassTypes.Teleport,
                        Characteristic = CharacteristicE.Speed,
                        Abilities = null,
                        PositionOnLine = 5,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Double, new PlayerGameClass
                    {
                        Name = "Double",
                        Type = ClassTypes.Double,
                        Characteristic = CharacteristicE.Intellect,
                        Abilities = null,
                        PositionOnLine = 5,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Cyborg, new PlayerGameClass
                    {
                        Name = "Cyborg",
                        Type = ClassTypes.Cyborg,
                        Characteristic = CharacteristicE.Endurance,
                        Abilities = null,
                        PositionOnLine = 5,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Implicator, new PlayerGameClass
                    {
                        Name = "Implicator",
                        Type = ClassTypes.Implicator,
                        Characteristic = CharacteristicE.Dexterity,
                        Abilities = null,
                        PositionOnLine = 5,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Paladin, new PlayerGameClass
                    {
                        Name = "Paladin",
                        Type = ClassTypes.Paladin,
                        Characteristic = CharacteristicE.Wisdom,
                        Abilities = null,
                        PositionOnLine = 5,
                        Sinergy = null,
                        Desc = "",
                        Tier = 2
                    }
                },
                {
                    ClassTypes.Exorcist, new PlayerGameClass
                    {
                        Name = "Exorcist",
                        Type = ClassTypes.Exorcist,
                        Characteristic = CharacteristicE.Power,
                        Abilities = null,
                        PositionOnLine = 6,
                        Sinergy = null,
                        Desc = "",
                        Tier = 3
                    }
                },
                {
                    ClassTypes.Resident, new PlayerGameClass
                    {
                        Name = "Resident",
                        Type = ClassTypes.Resident,
                        Characteristic = CharacteristicE.Speed,
                        Abilities = null,
                        PositionOnLine = 6,
                        Sinergy = null,
                        Desc = "",
                        Tier = 3
                    }
                },
                {
                    ClassTypes.Quantifier, new PlayerGameClass
                    {
                        Name = "Quantifier",
                        Type = ClassTypes.Quantifier,
                        Characteristic = CharacteristicE.Intellect,
                        Abilities = null,
                        PositionOnLine = 6,
                        Sinergy = null,
                        Desc = "",
                        Tier = 3
                    }
                },
                {
                    ClassTypes.Sintent, new PlayerGameClass
                    {
                        Name = "Sintent",
                        Type = ClassTypes.Sintent,
                        Characteristic = CharacteristicE.Endurance,
                        Abilities = null,
                        PositionOnLine = 6,
                        Sinergy = null,
                        Desc = "",
                        Tier = 3
                    }
                },
                {
                    ClassTypes.Sentinel, new PlayerGameClass
                    {
                        Name = "Sentinel",
                        Type = ClassTypes.Sentinel,
                        Characteristic = CharacteristicE.Dexterity,
                        Abilities = null,
                        PositionOnLine = 6,
                        Sinergy = null,
                        Desc = "",
                        Tier = 3
                    }
                },
                {
                    ClassTypes.Inquisitor, new PlayerGameClass
                    {
                        Name = "Inquisitor",
                        Type = ClassTypes.Inquisitor,
                        Characteristic = CharacteristicE.Wisdom,
                        Abilities = null,
                        PositionOnLine = 6,
                        Sinergy = null,
                        Desc = "",
                        Tier = 3
                    }
                }
            };

            ///////// FAKE INIT FOR ABILITY
            var x = 1;
            foreach (var gameClass in _classes)
            {
                if (gameClass.Key == ClassTypes.None) continue;

                gameClass.Value.Abilities = new PlayerClassAbility[6];
                for (var i = 0; i < 6; i++)
                {
                    gameClass.Value.Abilities[i] = new PlayerClassAbility
                    {
                        Name = x.ToString(),
                        Type = (AbilityTypeE)(i % 3),
                        Class = gameClass.Key,
                        Desc = "Description of " + x,
                    };
                    x++;
                }
            }
        }
    }
}
