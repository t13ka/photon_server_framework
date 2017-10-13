using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment.Armors;
using Warsmiths.Common.Results;
// ReSharper disable UseStringInterpolation

namespace Warsmiths.Common.Domain.Equipment
{
    public class BaseArmor : BaseEquipment
    {
        #region Props

        public string RecieptId ;

        public int AttackAngle
        {
            get
            {
                if (ArmorType == ArmorTypes.Heavy) return 60;
                if (ArmorType == ArmorTypes.Medium) return 120;
                if (ArmorType == ArmorTypes.Light) return 180;
                return 0;
            }
        }

        public int OverallTriggers ;
        public int ActiveTriggers ;
        public int OverallUpgrades ;
        public int ActiveUpgrades ;
        public int OverallModules ;
        public int ActiveModules ;
        public List<ArmorPart> ArmorParts ;
        public ArmorTypes ArmorType ;
        public StructureTypes StructureType ;
        public int FireShield ;
        public int IceShield ;
        public int ElectricityShield ;

        public List<TriggerTypes> Triggers;
        /// <summary>
        /// Вычисляемое кол-во модулей которое может быть вставленово все части
        /// </summary>
        public int ModulesCount
        {
            get { return ArmorParts.Sum(t => t.MaxModulesCount); }
        }

        /// <summary>
        /// Вычисляемая устойчивость модулей
        /// </summary>
        public int Casing
        {
            get { return (int)(ArmorParts.Sum(t => t.Casing) / (float)ArmorParts.Count); }
        }

        /// <summary>
        /// Вычисляемая Прочность модулей
        /// </summary>
        public int Durability
        {
            get { return ArmorParts.Sum(t => t.Durability); }
        }

        #endregion

        #region Ctors

        public BaseArmor(string name)
        {
            _id = Guid.NewGuid().ToString(); 
            Rarety = RaretyTypes.Regular;
            Triggers = new List<TriggerTypes>();
            ArmorParts = new List<ArmorPart>();
            Name = name;
        }

        public BaseArmor()
        {
            _id = Guid.NewGuid().ToString(); 
            Rarety = RaretyTypes.Regular;
            Triggers = new List<TriggerTypes>();
            ArmorParts = new List<ArmorPart>();
        }

        public override BaseEquipment Clone()
        {
            return new BaseArmor
            {
                ActiveModules = ActiveModules,
                ActiveTriggers = ActiveTriggers,
                ActiveUpgrades = ActiveUpgrades,
                ArmorParts = new List<ArmorPart>(ArmorParts),
                ArmorType = ArmorType,
                ElectricityShield = ElectricityShield,
                FireShield = FireShield,
                IceShield = IceShield,
                OverallModules = OverallModules,
                OverallTriggers = OverallTriggers,
                OverallUpgrades = OverallUpgrades,
                RecieptId = RecieptId,
                StructureType = StructureType,
                Triggers = Triggers,
                Name = Name,
                Sprite = Sprite,
                Rarety = Rarety,
                Chance = Chance,
                Anomality = Anomality,
                Price = Price,
                EnergyTypes = EnergyTypes,
                Weight = Weight,
                ColorType = ColorType,
                RaceType = RaceType,
                Prok = Prok,
                Sharpening = Sharpening,
                _id = _id,
            };
        }
        #endregion

        #region Methods

        public void InitializeArmorParts(int durability, int maxModulesCount, int casing)
        {
            ArmorParts = new List<ArmorPart>
            {
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.Back,
                    Durability = Convert.ToInt32(durability*0.25f),
                    Casing = casing,
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.Chest,
                    Durability = Convert.ToInt32(durability*0.25f),
                    Casing = casing,
                    MaxModulesCount = maxModulesCount,
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.RightHand,
                    Durability = Convert.ToInt32(durability*0.1f),
                    Casing = casing,
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.LeftHand,
                    Durability = Convert.ToInt32(durability*0.1f),
                    Casing = casing,
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.RightLeg,
                    Durability = Convert.ToInt32(durability*0.15f),
                    Casing = casing,
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.LeftLeg,
                    Durability = Convert.ToInt32(durability*0.15f),
                    Casing = casing,
                    Modules = new List<BaseModule>()
                },
            };

            ElectricityShield = Durability;
            FireShield = Durability;
            IceShield = Durability;
        }

        public void InitializeDefaultArmorParts()
        {
            var durability = 0;
            var casing = 0;
            var maxModulesCount = 0;

            if (ArmorType == ArmorTypes.Heavy)
            {
                durability = 600;
                casing = 40;
                Anomality = 20;
                maxModulesCount = 3;
            }

            if (ArmorType == ArmorTypes.Medium)
            {
                durability = 400;
                casing = 20;
                Anomality = 15;
                maxModulesCount = 2;
            }

            if (ArmorType == ArmorTypes.Light)
            {
                durability = 220;
                casing = 40;
                Anomality = 10;
                maxModulesCount = 1;
            }

            ArmorParts = new List<ArmorPart>
            {
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.Back,
                    Durability = Convert.ToInt32(durability*0.25f),
                    Casing = casing,//Convert.ToInt32(casing*0.25f),
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.Chest,
                    Durability = Convert.ToInt32(durability*0.25f),
                    Casing = casing,//Convert.ToInt32(casing*0.25f),
                    MaxModulesCount = maxModulesCount,
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.RightHand,
                    Durability = Convert.ToInt32(durability*0.1f),
                    Casing = casing,//Convert.ToInt32(casing*0.1f),
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.LeftHand,
                    Durability = Convert.ToInt32(durability*0.1f),
                    Casing = casing,//Convert.ToInt32(casing*0.1f),
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.RightLeg,
                    Durability = Convert.ToInt32(durability*0.15f),
                    Casing = casing,//Convert.ToInt32(casing*0.15f),
                    Modules = new List<BaseModule>()
                },
                new ArmorPart
                {
                    ArmorPartType = ArmorPartTypes.LeftLeg,
                    Durability = Convert.ToInt32(durability*0.15f),
                    Casing = casing,//Convert.ToInt32(casing*0.15f),
                    Modules = new List<BaseModule>()
                },
            };

            ElectricityShield = Durability;
            FireShield = Durability;
            IceShield = Durability;
        }

        public float GetRaretyModifier()
        {
            return 1.3f;
        }

        public float GetTotalPrice()
        {
            var modules = ArmorParts.Sum(t => t.Modules.Count);
            if (modules == 0)
                modules = 1;

            var resultPrice = Price
                              *(1 + 100f/modules/8)
                              *ModulesCount
                              *Casing
                              *0.025f/100f
                              *(1 + (Weight + Anomality)/100f);
            resultPrice = resultPrice - (resultPrice*Prok)/100;
            return resultPrice;
        }

       
        public override string ToString()
        {
            var durabilityString = new StringBuilder();
            var modulesString = new StringBuilder();
            var modulesCountString = new StringBuilder();

            if (ArmorParts == null)
                return "ARMOR: " + RaceType + " " + GetType().Name +
                       $" [Durability:{durabilityString}, TOTAL:{Durability}] " +
                       $" [ModulesStrenght:{modulesString}]; " +
                       $" [Modules count:{modulesCountString}]; " +
                       $" [Anomality:{Anomality}]; " +
                       $"Rarety:{Rarety}; Prok:{Prok}; Weight:{Weight}, Price: {Price}; Structure:{StructureType}; " +
                       $"[EnergyResist: red:{FireShield}; blue:{IceShield}; yellow:{ElectricityShield}]; color:{ColorType}; ";

            foreach (var armorPart in ArmorParts)
            {
                durabilityString.Append($" *{Math.Round(armorPart.GetPartOfValueByType(Durability), 2)}* ");
            }

            foreach (var armorPart in ArmorParts)
            {
                modulesString.Append($" *{armorPart.ArmorPartType}={Casing}* ");
            }

            foreach (var armorPart in ArmorParts)
            {
                modulesCountString.Append(armorPart.ArmorPartType == ArmorPartTypes.Chest
                    ? $" *{armorPart.ArmorPartType}={ModulesCount}* "
                    : $" *{armorPart.ArmorPartType}={0}* ");
            }

            return "ARMOR: " + RaceType + " " + GetType().Name +
                   $" [Durability:{durabilityString}, TOTAL:{Durability}] " +
                   $" [ModulesStrenght:{modulesString}]; " +
                   $" [Modules count:{modulesCountString}]; " +
                   $" [Anomality:{Anomality}]; " +
                   $"Rarety:{Rarety}; Prok:{Prok}; Weight:{Weight}, Price: {Price}; Structure:{StructureType}; " +
                   $"[EnergyResist: red:{FireShield}; blue:{IceShield}; yellow:{ElectricityShield}]; color:{ColorType}; ";
                // bonus:{Bonused}
        }

        public ModuleInsertRemoveOperationResult InsertModule(ArmorPartTypes armorPartType, BaseModule module)
        {
            if (ArmorParts == null)
            {
                return new ModuleInsertRemoveOperationResult
                {
                    Success = false,
                    Message = "ArmorParts is null"
                };
            }

            var armorPart = ArmorParts.FirstOrDefault(t => t.ArmorPartType == armorPartType);
            if (armorPart == null)
            {
                return new ModuleInsertRemoveOperationResult
                {
                    Success = false,
                    Message = "Armor part not found!"
                };
            }

            if (armorPart.Modules == null)
            {
                armorPart.Modules = new List<BaseModule>();
            }

            if (armorPart.Modules.Exists(t => t._id == module._id) == false)
            {
                if (armorPart.Modules.Count < armorPart.MaxModulesCount)
                {
                    armorPart.Modules.Add(module);
                    return new ModuleInsertRemoveOperationResult
                    {
                        Success = true,
                    };
                }
                else
                {
                    return new ModuleInsertRemoveOperationResult
                    {
                        Success = false,
                        Message = "You can't insert any modules into this part. Maximum count reached"
                    };
                }
            }
            else
            {
                return new ModuleInsertRemoveOperationResult
                {
                    Success = false,
                    Message = "Module already inserted"
                };
            }
        }

        public ModuleInsertRemoveOperationResult RemoveModule(string moduleId, out BaseModule takeOffModule)
        {
            takeOffModule = null;
            if (ArmorParts == null)
            {
                return new ModuleInsertRemoveOperationResult
                {
                    Success = false,
                    Message = "Parts is null!"
                };
            }

            var mod = ArmorParts.GroupBy(t => new
            {
                part = t,
                modules = t.Modules
            });

            foreach (var g in mod)
            {
                if (g.Key.modules != null)
                {
                    if (g.Key.modules.Exists(m => m._id == moduleId))
                    {
                        takeOffModule = g.Key.modules.First(t => t._id == moduleId);

                        if (takeOffModule.Sealed)
                        {
                            return new ModuleInsertRemoveOperationResult
                            {
                                Success = true,
                                Message = "Can't remove module, because it is Sealed"
                            };
                        }
                        else
                        {
                            g.Key.modules.Remove(takeOffModule);

                            return new ModuleInsertRemoveOperationResult
                            {
                                Success = true
                            };
                        }
                    }
                }
            }

            return new ModuleInsertRemoveOperationResult
            {
                Success = false,
                Message = "Module for remove is not found!"
            };
        }

        #endregion
    }
}