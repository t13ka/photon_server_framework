using System;
using System.Collections.Generic;
using Warsmiths.Common.Domain.CommonCharacterProfile.Others;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Tasks;
using Warsmiths.Common.Results;
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable LoopCanBeConvertedToQuery

namespace Warsmiths.Common.Domain
{
    public class Character : IEntity
    {
        #region Fields
        public CommonCharacterProfile.CommonCharacterProfile CommonProfile;
        
        public string[] RageAbilities = new string[6];
        public string[] MoraleAbilities = new string[6];
        public string[] PassiveAbilities = new string[6];
        #endregion

        #region Props
        public RaceTypes RaceType ;

        public HeroTypes HeroType ;

        public int Level
        {
            get
            {
                var level = 1;
                for (var i = 1; i < DomainConfiguration.CharacterLevelsExperience.Length; i++)
                {
                    var entry = DomainConfiguration.CharacterLevelsExperience[i];
                    if (Experience >= entry)
                    {
                        level++;
                    }
                }
                return level;
            }
        }

        /// <summary>
        /// Максимально допустимое количество наемников в бою
        /// </summary>
        public int MaxAllyForce;
        public int Experience ;

        public int CraftExperience ;

        public int CraftLevel ;

        public bool Selected ;

        public CharacterEquipment Equipment ;

        public int ClassPoints ;
        public int AbilityPoints ;
        public List<ClassTypes> Classes ;

        public List<string> Reciepts ;
        public List<string> CompletedQuests ;
        public List<IEntity> TasksList ;
        #endregion

        #region Ctor

        public Character()
        {
            _id = Guid.NewGuid().ToString();
            CommonProfile = new CommonCharacterProfile.CommonCharacterProfile();
            CommonProfile.Init(this);
            Experience = 0;
            CraftLevel = 0;
            TasksList = new List<IEntity>();
            Equipment = new CharacterEquipment();
            Classes = new List<ClassTypes>();
            Reciepts = new List<string>();
            CompletedQuests = new List<string>();
            AbilityPoints = 3;
            
            CommonProfile.Calculate();
        }

        #endregion

        #region Methods
        /// <summary>
        /// </summary>
        public void Update()
        {
            if (CommonProfile == null)
            {
                CommonProfile = new CommonCharacterProfile.CommonCharacterProfile();
            }
            CommonProfile.Init(this);
            CommonProfile.Calculate();
        }

        

        public bool AddAbility(int position, string abiName)
        {
            if (AbilityPoints <= 0) return false;

            var abi = PlayerAbilityCollection.Abilities[abiName];
            var targetAbi = abi.Type == AbilityTypeE.Rage
                ? RageAbilities
                : (abi.Type == AbilityTypeE.Morale ? MoraleAbilities : PassiveAbilities);

            var cls = PlayerClassCollection.Classes[abi.Class];
            var minPos = cls.Tier == 1 ? 0 : cls.Tier == 2 ? 2 : 4;
            if (position != -1 && position < minPos) return false;
            var tIndex = Array.IndexOf(targetAbi, abiName);
            if (tIndex != -1)
            {
                targetAbi[tIndex] = null;
                // Сделано для корректного учета переноса абилки с нового места на старое
                AbilityPoints++;
            }

            if (position == -1)
            {
                for (var i = minPos; i < targetAbi.Length; i++)
                {
                    if (!string.IsNullOrEmpty(targetAbi[i])) continue;
                    position = i;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(targetAbi[position]))
            {
                // так как мы при установки новой абилки в уже занятую ячейку должны "сбросить" старую абилку - AbilityPoints не должен меняться
                AbilityPoints++;
            }
            targetAbi[position] = abiName;
            AbilityPoints--;
            return true;
        }

        public bool RemoveAbility(string abiName)
        {
            var aIndex = Array.IndexOf(RageAbilities, abiName);
            if (aIndex != -1)
            {
                RageAbilities[aIndex] = null;
                AbilityPoints++;
                return true;
            }

            aIndex = Array.IndexOf(MoraleAbilities, abiName);
            if (aIndex != -1)
            {
                MoraleAbilities[aIndex] = null;
                AbilityPoints++;
                return true;
            }

            aIndex = Array.IndexOf(PassiveAbilities, abiName);
            if (aIndex != -1)
            {
                PassiveAbilities[aIndex] = null;
                AbilityPoints++;
                return true;
            }
            return false;
        }


        public AddClassResult AddClass(ClassTypes classType)
        {
            AddClassResult result;
            if (ClassPoints > 0)
            {
                PlayerGameClass playerGameClass;

                if (PlayerClassCollection.Classes.TryGetValue(classType, out playerGameClass) == false)
                {
                    return new AddClassResult
                    {
                        Debug = "class not found! Server error",
                        Success = false
                    };
                }

                if (Classes.Exists(t => t == classType))
                {
                    return new AddClassResult
                    {
                        Debug = "this class already added. " + classType,
                        Success = false
                    };
                }

                var c = playerGameClass;
                var characteristic = playerGameClass.Characteristic;
                switch (characteristic)
                {
                    case CharacteristicE.Power:
                        if (c.Tier == 1 || c.Tier == 3)
                        {
                            CommonProfile.Characteristics.Power.Strenght += 25;
                        }
                        else
                        {
                            CommonProfile.Characteristics.Wisdom.Strenght += 25;
                        }
                        break;

                    case CharacteristicE.Speed:
                        if (c.Tier == 1 || c.Tier == 3)
                        {
                            CommonProfile.Characteristics.Speed.Strenght += 25;
                        }
                        else
                        {
                            CommonProfile.Characteristics.Dexterity.Strenght += 25;
                        }
                        break;

                    case CharacteristicE.Intellect:
                        if (c.Tier == 1 || c.Tier == 3)
                        {
                            CommonProfile.Characteristics.Intellect.Strenght += 25;
                        }
                        else
                        {
                            CommonProfile.Characteristics.Endurance.Strenght += 25;
                        }
                        break;

                    case CharacteristicE.Endurance:
                        if (c.Tier == 1 || c.Tier == 3)
                        {
                            CommonProfile.Characteristics.Endurance.Strenght += 25;
                        }
                        else
                        {
                            CommonProfile.Characteristics.Intellect.Strenght += 25;
                        }
                        break;

                    case CharacteristicE.Dexterity:
                        if (c.Tier == 1 || c.Tier == 3)
                        {
                            CommonProfile.Characteristics.Dexterity.Strenght += 25;
                        }
                        else
                        {
                            CommonProfile.Characteristics.Speed.Strenght += 25;
                        }
                        break;

                    case CharacteristicE.Wisdom:
                        if (c.Tier == 1 || c.Tier == 3)
                        {
                            CommonProfile.Characteristics.Wisdom.Strenght += 25;
                        }
                        else
                        {
                            CommonProfile.Characteristics.Power.Strenght += 25;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(characteristic), characteristic, null);
                }
                
                ClassPoints--;
                Classes.Add(classType);
                result = new AddClassResult
                {
                    Success = true,
                    Debug = "class added"
                };
                CommonProfile.Calculate();
            }
            else
            {
                result = new AddClassResult
                {
                    Success = false,
                    Debug = "not enought class points"
                };
            }

            return result;
        }

        public void SetCharacteristicSkillPercern(CharacteristicE characteristic, int skillPercent)
        {
            switch (characteristic)
            {
                case CharacteristicE.Power:
                    CommonProfile.Characteristics.Power.Percent = skillPercent;
                    break;

                case CharacteristicE.Speed:
                    CommonProfile.Characteristics.Speed.Percent = skillPercent;
                    break;
                case CharacteristicE.Intellect:
                    CommonProfile.Characteristics.Intellect.Percent = skillPercent;

                    break;
                case CharacteristicE.Endurance:
                    CommonProfile.Characteristics.Endurance.Percent = skillPercent;

                    break;
                case CharacteristicE.Dexterity:
                    CommonProfile.Characteristics.Dexterity.Percent = skillPercent;
                    break;
                case CharacteristicE.Wisdom:
                    CommonProfile.Characteristics.Wisdom.Percent = skillPercent;
                    break;
            }
            CommonProfile.Calculate();
        }

        public void SetExperience(int exp)
        {
            var currentLevel = Level;

            Experience = exp;
            var level = Level;
            if (level > 3)
            {
                if (level > 3 && level < 7)
                {
                    ClassPoints = 1;
                }
                else if (level >= 7 && level < 10)
                {
                    ClassPoints = 2;
                }
                else if (level >= 10 && level < 13)
                {
                    ClassPoints = 3;
                }
                else
                {
                    ClassPoints = 4;
                }
            }

            var abiPoints = level - currentLevel;
            AbilityPoints = AbilityPoints + abiPoints;

            CommonProfile.Calculate();
        }
        #endregion
    }
}