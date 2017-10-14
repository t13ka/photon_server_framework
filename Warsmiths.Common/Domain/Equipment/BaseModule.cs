
// ReSharper disable SwitchStatementMissingSomeCases

namespace YourGame.Common.Domain.Equipment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YourGame.Common.Domain.Enums;
    using YourGame.Common.Domain.Enums.ItemGeneration;
    using YourGame.Common.Utils;

    public enum ModuleInsertToE
    {
        Hands = 0,
        Legs = 1,
        Chest = 2,
        Back = 3
    }

    public enum ModuleRelevanceTypeE
    {
        Regular = 0,
        Middle = 1,
        Important = 2
    }

    public class BaseModule : BaseEquipment
    {
        #region Fields

        private static readonly Dictionary<ModulesTypesOfImpacts, ItemGenerationLuckTypes> ChancesGroup = new Dictionary
            <ModulesTypesOfImpacts, ItemGenerationLuckTypes>
        {
            {ModulesTypesOfImpacts.MeleeAttackSpeed, ItemGenerationLuckTypes.Legend},
            {ModulesTypesOfImpacts.MeleeAttackDistance , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.MeleeMaxTargets , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.MeleeDisruptionMod , ItemGenerationLuckTypes.Regular},
            {ModulesTypesOfImpacts.MeleeCriticalMod , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.MeleeWeight , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.MeleeAnomality , ItemGenerationLuckTypes.Epic},

            {ModulesTypesOfImpacts.RangeAttackSpeed , ItemGenerationLuckTypes.Legend},
            {ModulesTypesOfImpacts.RangeDamageRadius , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.RangeAttackRange , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.RangeDisruptionMod , ItemGenerationLuckTypes.Regular},
            {ModulesTypesOfImpacts.RangeCriticalMod , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.RangeWeight , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.RangeAnomality , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.RangeAccuracy , ItemGenerationLuckTypes.Regular},
            {ModulesTypesOfImpacts.RangeShotInLine , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.RangeHolder , ItemGenerationLuckTypes.Epic},

            {ModulesTypesOfImpacts.MagicAttackSpeed , ItemGenerationLuckTypes.Legend},
            {ModulesTypesOfImpacts.MagicMaxTargets , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.MagicChargeVelocity , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.MagicDisruptionMod , ItemGenerationLuckTypes.Regular},
            {ModulesTypesOfImpacts.MagicCriticalMod , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.MagicCharger , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.MagicWeight , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.MagicAnomality , ItemGenerationLuckTypes.Epic},

            {ModulesTypesOfImpacts.MeleeDefenceAbilityResist , ItemGenerationLuckTypes.Regular},
            {ModulesTypesOfImpacts.MeleeDefenceAttackAngle , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.MeleeDefenceGeneralCasing , ItemGenerationLuckTypes.Legend},

            {ModulesTypesOfImpacts.RangeDefenceMovementResist , ItemGenerationLuckTypes.Regular},

            {ModulesTypesOfImpacts.MagicDefenceSpiritResist , ItemGenerationLuckTypes.Regular},

            {ModulesTypesOfImpacts.MindImmunity , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.MindInitiative , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.MindStamina , ItemGenerationLuckTypes.Rare},

            {ModulesTypesOfImpacts.BodyMovement , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.BodyCharge , ItemGenerationLuckTypes.Rare},
            {ModulesTypesOfImpacts.BodyVision , ItemGenerationLuckTypes.Rare},

            {ModulesTypesOfImpacts.PowerValue , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.SpeedValue , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.IntellectValue , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.EnduranceValue , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.DexteretyValue , ItemGenerationLuckTypes.Epic},
            {ModulesTypesOfImpacts.WisdomValue, ItemGenerationLuckTypes.Epic},
        };

        private static readonly Dictionary<ModulesTypesOfImpacts, ModuleRelevanceTypeE> Relevances = new Dictionary
            <ModulesTypesOfImpacts, ModuleRelevanceTypeE>
        {
            {ModulesTypesOfImpacts.MeleeAttackSpeed, ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.MeleeAttackDistance , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MeleeMaxTargets , ModuleRelevanceTypeE.Regular},
            {ModulesTypesOfImpacts.MeleeDisruptionMod , ModuleRelevanceTypeE.Regular},
            {ModulesTypesOfImpacts.MeleeCriticalMod , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MeleeWeight , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MeleeAnomality , ModuleRelevanceTypeE.Middle},

            {ModulesTypesOfImpacts.RangeAttackSpeed , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.RangeDamageRadius , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.RangeAttackRange , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.RangeDisruptionMod , ModuleRelevanceTypeE.Regular},
            {ModulesTypesOfImpacts.RangeCriticalMod , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.RangeWeight , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.RangeAnomality , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.RangeAccuracy , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.RangeShotInLine , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.RangeHolder , ModuleRelevanceTypeE.Middle},

            {ModulesTypesOfImpacts.MagicAttackSpeed , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.MagicMaxTargets , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MagicChargeVelocity , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MagicDisruptionMod , ModuleRelevanceTypeE.Regular},
            {ModulesTypesOfImpacts.MagicCriticalMod , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MagicCharger , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MagicWeight , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.MagicAnomality , ModuleRelevanceTypeE.Middle},

            {ModulesTypesOfImpacts.MeleeDefenceAbilityResist , ModuleRelevanceTypeE.Regular},
            {ModulesTypesOfImpacts.MeleeDefenceAttackAngle , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.MeleeDefenceGeneralCasing , ModuleRelevanceTypeE.Important},

            {ModulesTypesOfImpacts.RangeDefenceMovementResist , ModuleRelevanceTypeE.Regular},

            {ModulesTypesOfImpacts.MagicDefenceSpiritResist , ModuleRelevanceTypeE.Regular},

            {ModulesTypesOfImpacts.MindImmunity , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.MindInitiative , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.MindStamina , ModuleRelevanceTypeE.Important},

            {ModulesTypesOfImpacts.BodyMovement , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.BodyCharge , ModuleRelevanceTypeE.Important},
            {ModulesTypesOfImpacts.BodyVision , ModuleRelevanceTypeE.Important},

            {ModulesTypesOfImpacts.PowerValue , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.SpeedValue , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.IntellectValue , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.EnduranceValue , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.DexteretyValue , ModuleRelevanceTypeE.Middle},
            {ModulesTypesOfImpacts.WisdomValue, ModuleRelevanceTypeE.Middle},
        };

        public static readonly Dictionary<ModulesTypesOfImpacts, List<ModuleInsertToE>> AllowedPlaces = new Dictionary
            <ModulesTypesOfImpacts, List<ModuleInsertToE>>
        {
            {ModulesTypesOfImpacts.MeleeAttackSpeed, new List<ModuleInsertToE>{ModuleInsertToE.Hands}},
            {ModulesTypesOfImpacts.MeleeAttackDistance , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Chest}},
            {ModulesTypesOfImpacts.MeleeMaxTargets , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.MeleeDisruptionMod , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.MeleeCriticalMod , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.MeleeWeight , new List<ModuleInsertToE>{ModuleInsertToE.Back, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.MeleeAnomality , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Chest}},

            {ModulesTypesOfImpacts.RangeAttackSpeed , new List<ModuleInsertToE>{ModuleInsertToE.Hands}},
            {ModulesTypesOfImpacts.RangeDamageRadius , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.RangeAttackRange , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Chest}},
            {ModulesTypesOfImpacts.RangeDisruptionMod , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.RangeCriticalMod , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.RangeWeight , new List<ModuleInsertToE>{ModuleInsertToE.Back, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.RangeAnomality , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Chest}},
            {ModulesTypesOfImpacts.RangeAccuracy , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.RangeShotInLine , new List<ModuleInsertToE>{ModuleInsertToE.Back, ModuleInsertToE.Chest}},
            {ModulesTypesOfImpacts.RangeHolder , new List<ModuleInsertToE>{ModuleInsertToE.Back, ModuleInsertToE.Hands}},

            {ModulesTypesOfImpacts.MagicAttackSpeed , new List<ModuleInsertToE>{ModuleInsertToE.Hands}},
            {ModulesTypesOfImpacts.MagicMaxTargets , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.MagicChargeVelocity , new List<ModuleInsertToE>{ModuleInsertToE.Back, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.MagicDisruptionMod , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.MagicCriticalMod , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.MagicCharger , new List<ModuleInsertToE>{ModuleInsertToE.Back, ModuleInsertToE.Hands}},
            {ModulesTypesOfImpacts.MagicWeight , new List<ModuleInsertToE>{ModuleInsertToE.Back, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.MagicAnomality , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Chest}},

            {ModulesTypesOfImpacts.MeleeDefenceAbilityResist , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.MeleeDefenceAttackAngle , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.MeleeDefenceGeneralCasing , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},

            {ModulesTypesOfImpacts.RangeDefenceMovementResist , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},

            {ModulesTypesOfImpacts.MagicDefenceSpiritResist , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},

            {ModulesTypesOfImpacts.MindImmunity , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.MindInitiative , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.MindStamina , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},

            {ModulesTypesOfImpacts.BodyMovement , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.BodyCharge , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},
            {ModulesTypesOfImpacts.BodyVision , new List<ModuleInsertToE>{ModuleInsertToE.Chest, ModuleInsertToE.Legs, ModuleInsertToE.Back}},

            {ModulesTypesOfImpacts.PowerValue , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.SpeedValue , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.IntellectValue , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.EnduranceValue , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.DexteretyValue , new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Legs}},
            {ModulesTypesOfImpacts.WisdomValue, new List<ModuleInsertToE>{ModuleInsertToE.Hands, ModuleInsertToE.Legs}},
        };

        #endregion

        #region Props

        public ModuleRelevanceTypeE Relevance => Relevances[TypesOfImpact];

        /// <summary>
        /// Тип воздействия
        /// </summary>
        public ModulesTypesOfImpacts TypesOfImpact ;

        public ItemGenerationLuckTypes Group => ChancesGroup[TypesOfImpact];

        public ModuleInsertToE InsertTo;
        /// <summary>
        /// Флаг зачеченности, запечатанности.
        /// </summary>
        public bool Sealed ;

        public ModuleInsertToE GetRandomAvaliablePlace()
        {
            var l = AllowedPlaces[TypesOfImpact];
            return l[DomainConfiguration.Random.Next(l.Count)];
        }

        /// <summary>
        /// значение влияния модуля
        /// </summary>
        public int Value
        {
            get
            {
                switch (Relevance)
                {
                    case ModuleRelevanceTypeE.Regular:
                        switch (Rarety)
                        {
                            case RaretyTypes.Regular:
                                return 5;
                            case RaretyTypes.Rare:
                                return 10;
                            case RaretyTypes.Epic:
                                return 20;
                            case RaretyTypes.Legend:
                                return 30;
                        }
                        break;
                    case ModuleRelevanceTypeE.Middle:
                        switch (Rarety)
                        {
                            case RaretyTypes.Regular:
                                return 3;
                            case RaretyTypes.Rare:
                                return 5;
                            case RaretyTypes.Epic:
                                return 10;
                            case RaretyTypes.Legend:
                                return 15;
                        }
                        break;
                    case ModuleRelevanceTypeE.Important:
                        switch (Rarety)
                        {
                            case RaretyTypes.Regular:
                                return 2;
                            case RaretyTypes.Rare:
                                return 4;
                            case RaretyTypes.Epic:
                                return 7;
                            case RaretyTypes.Legend:
                                return 10;
                        }
                        break;
                }
                return 0;
            }
        }



        #endregion

        public override BaseEquipment Clone()
        {
            return new BaseModule
            {
                Sealed = Sealed,
                TypesOfImpact = TypesOfImpact,
                Name = Name,
                Sprite = Sprite,
                Rarety = Rarety,
                Anomality = Anomality,
                Weight = Weight,
                RaceType = RaceType,
                _id = _id,
                Prok = Prok,
                Price = Price,
                ColorType = ColorType,
                EnergyTypes = EnergyTypes,
                Sharpening = Sharpening,
            };
        }

        #region Ctors
        public BaseModule()
        {
            _id = Guid.NewGuid().ToString();
            Price = 300;
        }
        #endregion

        public int ModuleAnomality
        {
            get
            {
                switch (Rarety)
                {
                    case RaretyTypes.Regular:
                        return 5;
                    case RaretyTypes.Rare:
                        return 10;
                    case RaretyTypes.Epic:
                        return 25;
                    case RaretyTypes.Legend:
                        return 50;
                }
                return 5;
            }
        }

        public int ModuleCapacity
        {
            get
            {

                switch (Rarety)
                {
                    case RaretyTypes.Regular:
                        return 1;
                    case RaretyTypes.Rare:
                        return 2;
                    case RaretyTypes.Epic:
                        return 5;
                    case RaretyTypes.Legend:
                        return 10;
                }
                return 5;
            }
        }

        #region Methods
        public void RandomizeImpactProperty(ItemGenerationLuckTypes luck)
        {
            // random
            var rnd = DomainConfiguration.Random.Next(100);
            var generatedGroup = ItemGenerationLuckTypes.Regular;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (luck)
            {
                case ItemGenerationLuckTypes.Regular:
                    generatedGroup = rnd < 90 ? ItemGenerationLuckTypes.Regular : ItemGenerationLuckTypes.Rare;
                    break;
                case ItemGenerationLuckTypes.Rare:
                    if (rnd < 25)
                    {
                        generatedGroup = ItemGenerationLuckTypes.Regular;
                    }
                    else
                    {
                        generatedGroup = rnd < 90 ? ItemGenerationLuckTypes.Rare : ItemGenerationLuckTypes.Epic;
                    }
                    break;
                case ItemGenerationLuckTypes.Epic:
                    if (rnd < 25)
                    {
                        generatedGroup = ItemGenerationLuckTypes.Rare;
                    }
                    else
                    {
                        generatedGroup = (rnd < 90) ? ItemGenerationLuckTypes.Epic : ItemGenerationLuckTypes.Legend;
                    }
                    break;
                case ItemGenerationLuckTypes.Legend:
                    generatedGroup = rnd < 40 ? ItemGenerationLuckTypes.Epic : ItemGenerationLuckTypes.Legend;
                    break;
            }
            var values = ChancesGroup.Where(t => t.Value == generatedGroup).ToArray();
            var idx = DomainConfiguration.Random.Next(values.Length);
            var randomImpact = values[idx].Key;
            TypesOfImpact = randomImpact;
            Name = TypesOfImpact.MakeName();
        }

        public float GetTotalPrice()
        {
            var raretyMod = 1;
            /*if (Rarety == RaretyTypes.Rare)
            {
                raretyMod = 5;
            }
            if (Rarety == RaretyTypes.Epic)
            {
                raretyMod = 10;
            }
            if (Rarety == RaretyTypes.Legend)
            {
                raretyMod = 20;
            }*/

            return Price - Prok * raretyMod;
        }

        public override string ToString()
        {
            return "MODULE:" + Name + $" ({TypesOfImpact}: {Value}%; Rarety: {Rarety}; Relevance: {Relevance}; Chance: {Group}; Insert To: {InsertTo}) Price: {Price}";
        }
        #endregion
    }
}