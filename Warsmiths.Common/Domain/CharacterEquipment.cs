using System;
using System.Collections.Generic;
using System.Linq;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Results;
// ReSharper disable UseNullPropagation
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable LoopCanBeConvertedToQuery

namespace Warsmiths.Common.Domain
{
    public class CharacterEquipment
    {
        public BaseArmor Armor;
        public BaseWeapon RightWeapon;
        public BaseWeapon LeftWeapon;

        #region Methods

        public WearingResult PutOn(BaseEquipment equipment, EquipmentPlaceTypes place)
        {
            WearingResult result;

            var checkResult = CheckWearAvailibility(equipment, place);
            if (checkResult.Success)
            {
                var takeOffList = WhatNeedTakeOff(equipment, checkResult.Place);

                result = new WearingResult
                {
                    Success = true,
                    TakeOffEquipments = TakeOffEqipmentFromPlaces(takeOffList)
                };

                switch (checkResult.Place)
                {
                    case EquipmentPlaceTypes.Armor:
                        Armor = equipment as BaseArmor;
                        break;
                    case EquipmentPlaceTypes.LeftHand:
                        LeftWeapon = equipment as BaseWeapon;
                        break;
                    case EquipmentPlaceTypes.RightHand:
                        RightWeapon = equipment as BaseWeapon;
                        break;
                }
            }
            else
            {
                result = new WearingResult
                {
                    Success = false,
                    TakeOffEquipments = new List<BaseEquipment>()
                };
            }

            return result;
        }

        public BaseEquipment TakeOff(EquipmentPlaceTypes place)
        {
            BaseEquipment equipment = null;

            switch (place)
            {
                case EquipmentPlaceTypes.None:
                    break;
                case EquipmentPlaceTypes.Armor:
                    equipment = Armor;
                    Armor = null;
                    break;
                case EquipmentPlaceTypes.LeftHand:
                    equipment = LeftWeapon;
                    LeftWeapon = null;
                    break;
                case EquipmentPlaceTypes.RightHand:
                    equipment = RightWeapon;
                    RightWeapon = null;
                    break;
            }
            return equipment;
        }

        private List<BaseEquipment> TakeOffEqipmentFromPlaces(IEnumerable<EquipmentPlaceTypes> takeOffPlaces)
        {
            var result = new List<BaseEquipment>();

            foreach (var place in takeOffPlaces)
            {
                var item = TakeOff(place);
                if (item != null)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private IEnumerable<EquipmentPlaceTypes> WhatNeedTakeOff(BaseEquipment equipment, EquipmentPlaceTypes place)
        {
            var list = new List<EquipmentPlaceTypes>();
            var armor = equipment as BaseArmor;
            var weapon = equipment as BaseWeapon;

            switch (place)
            {
                case EquipmentPlaceTypes.None:
                    break;

                case EquipmentPlaceTypes.Armor:
                    if (armor != null)
                    {
                        list.Add(EquipmentPlaceTypes.Armor);
                    }
                    break;

                case EquipmentPlaceTypes.LeftHand:
                    if (weapon != null)
                    {
                        if (weapon.WeaponHands == WeaponHandsTypes.OneHand)
                        {
                            list.Add(EquipmentPlaceTypes.LeftHand);
                        }
                    }
                    break;

                case EquipmentPlaceTypes.RightHand:
                    if (weapon != null)
                    {
                        if (weapon.WeaponHands == WeaponHandsTypes.TwoHand)
                        {
                            list.Add(EquipmentPlaceTypes.LeftHand);
                            list.Add(EquipmentPlaceTypes.RightHand);
                        }
                        else
                        {
                            list.Add(EquipmentPlaceTypes.RightHand);
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(place), place, null);
            }

            return list;
        }

        private CheckAvailPutOnResult CheckWearAvailibility(BaseEquipment equipment, EquipmentPlaceTypes place)
        {
            const EquipmentPlaceTypes rhp = EquipmentPlaceTypes.RightHand;
            const EquipmentPlaceTypes lhp = EquipmentPlaceTypes.LeftHand;
            const EquipmentPlaceTypes armor = EquipmentPlaceTypes.Armor;

            var fail = new CheckAvailPutOnResult { Success = false };

            // none
            switch (place)
            {
                case lhp:
                case rhp:
                    var equipmentWeapon = equipment as BaseWeapon;
                    // Проверяем что одеваем оружие и это не щит
                    if (equipmentWeapon != null && equipmentWeapon.WeaponSize != WeaponSizeTypes.Shield)
                    {
                        // проверяем что одеваем двуручку, если да - то окей, вставляем в правую
                        if (equipmentWeapon.WeaponHands == WeaponHandsTypes.TwoHand)
                        {
                            return new CheckAvailPutOnResult
                            {
                                Success = true,
                                Place = EquipmentPlaceTypes.RightHand
                            };
                        }
                        // если оружие только в правой
                        if (RightWeapon != null && LeftWeapon == null)
                        {
                            if (place == rhp)
                            {
                                return new CheckAvailPutOnResult
                                {
                                    Success = true,
                                    Place = place
                                };
                            }
                            {
                                if (RightWeapon.WeaponHands == WeaponHandsTypes.TwoHand) return fail;

                                if ((equipmentWeapon.WeaponType == WeaponTypes.Magic && RightWeapon.WeaponType == WeaponTypes.Ranged) ||
                                    (equipmentWeapon.WeaponType == WeaponTypes.Ranged && RightWeapon.WeaponType == WeaponTypes.Magic))
                                {
                                    return fail;
                                }
                                return new CheckAvailPutOnResult
                                {
                                    Success = true,
                                    Place = EquipmentPlaceTypes.LeftHand
                                };
                            }
                        }

                        // если оружия в обоих руках
                        if (RightWeapon != null && LeftWeapon != null)
                        {
                            if (place == rhp)
                            {
                                if ((equipmentWeapon.WeaponType == WeaponTypes.Magic && LeftWeapon.WeaponType == WeaponTypes.Ranged) ||
                                    (equipmentWeapon.WeaponType == WeaponTypes.Ranged && LeftWeapon.WeaponType == WeaponTypes.Magic))
                                {
                                    return fail;
                                }
                                return new CheckAvailPutOnResult
                                {
                                    Success = true,
                                    Place = EquipmentPlaceTypes.RightHand
                                };
                            }
                            if (place == lhp)
                            {
                                if (RightWeapon.WeaponHands == WeaponHandsTypes.TwoHand) return fail;

                                if ((equipmentWeapon.WeaponType == WeaponTypes.Magic && RightWeapon.WeaponType == WeaponTypes.Ranged) ||
                                    (equipmentWeapon.WeaponType == WeaponTypes.Ranged && RightWeapon.WeaponType == WeaponTypes.Magic))
                                {
                                    return fail;
                                }
                                return new CheckAvailPutOnResult
                                {
                                    Success = true,
                                    Place = EquipmentPlaceTypes.LeftHand
                                };
                            }
                        }
                    }
                    else if (equipmentWeapon != null && equipmentWeapon.WeaponSize == WeaponSizeTypes.Shield)
                    {
                        if (place == lhp && RightWeapon != null && RightWeapon.WeaponHands != WeaponHandsTypes.TwoHand)
                        {
                            return new CheckAvailPutOnResult
                            {
                                Success = true,
                                Place = EquipmentPlaceTypes.LeftHand
                            };
                        }
                    }
                    break;

                case armor:
                    if (equipment is BaseArmor)
                    {
                        return new CheckAvailPutOnResult
                        {
                            Success = true,
                            Place = EquipmentPlaceTypes.Armor,
                        };
                    }
                    break;
            }

            return fail;
        }

        public float GetArmorCoeficient<T>()
        {
            var c = 1f;

            var equipment = Armor;

            if (equipment != null)
            {
                if (equipment is T)
                {
                    c = 1.5f;
                }
            }
            else
            {
                c = 1;
            }

            return c;
        }

        /// <summary>
        /// Поправка на уровень предмета. В данном случае брони. Нужно Отрефакторить
        /// </summary>
        /// <param name="characterLevel"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public float GetArmorIncreasionDependedFromLevel(int characterLevel, float coef)
        {
            var result = 0f;
            if (Armor != null)
            {
                if (characterLevel >= 0)
                {
                    result = coef * DomainConfiguration.EquipmentLevelPercents[characterLevel] / 100;
                }
            }
            return result;
        }

        public float TotalAnomality()
        {
            var commonDevider = 0;

            var weapA = 0f;
            var armA = 0f;
            var modA = 0f;

            if (RightWeapon != null)
            {
                weapA += RightWeapon.Anomality;
            }

            if (LeftWeapon != null)
            {
                weapA += LeftWeapon.Anomality;
                weapA /= 2f;
            }

            commonDevider++;

            if (Armor != null)
            {
                armA = Armor.Anomality;
                commonDevider++;

                var modules = Armor.ArmorParts.SelectMany(t => t.Modules).Where(t => t != null).ToList();

                if (modules.Count > 0)
                {
                    modA = (float)modules.Sum(t => t.ModuleAnomality) / modules.Count;
                    commonDevider++;
                }
            }

            return (weapA + armA + modA) / commonDevider;
        }

        public float TotalWeight()
        {
            var a = Armor?.Weight ?? 0;
            var l = LeftWeapon?.Weight ?? 0;
            var r = RightWeapon?.Weight ?? 0;
            var result = a + l + r;

            if (Armor != null)
            {
                // TODO: Add modules support
                /*var modules = Armor.ArmorParts.SelectMany(t => t.Modules)
                    .Where(t => t != null && t.TypesOfImpact == ModulesTypesOfImpacts.Weight).ToList();

                result = result + (float)modules.Sum(t => t.Value);*/
            }
            return result;
        }

        public ResultAttribute<float> GetMeleeDamage(Character c, WeaponTypes wp)
        {
            var result = new ResultAttribute<float>();
            var weaponInRightHand = RightWeapon;
            var weaponInLeftHand = LeftWeapon;

            if (weaponInRightHand != null && weaponInRightHand.WeaponType == wp)
            {
                result.RightHandValue = weaponInRightHand.GetMeleeDamage(c);
            }
            if (weaponInLeftHand != null && weaponInLeftHand.WeaponType == wp)
            {
                result.LeftHandValue = weaponInLeftHand.GetMeleeDamage(c);
            }
            return result;
        }

        public ResultAttribute<float> GetMeleeDamage(Character c)
        {
            var result = new ResultAttribute<float>();
            var weaponInRightHand = RightWeapon;
            var weaponInLeftHand = LeftWeapon;

            if (weaponInRightHand != null)
            {
                result.RightHandValue = weaponInRightHand.GetMeleeDamage(c);
            }
            if (weaponInLeftHand != null)
            {
                result.LeftHandValue = weaponInLeftHand.GetMeleeDamage(c);
            }
            return result;
        }

        public ResultAttribute<float> GetDamageByWeaponType(Character c, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInRightHand = RightWeapon;
            var weaponInLeftHand = LeftWeapon;

            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    switch (takedDamageWeaponType)
                    {
                        case WeaponTypes.None:
                        case WeaponTypes.Melee:
                            result.RightHandValue = weaponInRightHand.GetMeleeDamage(c);
                            break;
                        case WeaponTypes.Ranged:
                            result.RightHandValue = weaponInRightHand.GetRangeDamage(c);
                            break;
                        case WeaponTypes.Magic:
                            result.RightHandValue = weaponInRightHand.GetMagicDamage(c);
                            break;
                    }
                }
            }

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    switch (takedDamageWeaponType)
                    {
                        case WeaponTypes.None:
                        case WeaponTypes.Melee:
                            result.LeftHandValue = weaponInLeftHand.GetMeleeDamage(c);
                            break;
                        case WeaponTypes.Ranged:
                            result.LeftHandValue = weaponInLeftHand.GetRangeDamage(c);
                            break;
                        case WeaponTypes.Magic:
                            result.LeftHandValue = weaponInLeftHand.GetMagicDamage(c);
                            break;
                    }
                }
            }

            return result;
        }

        public class ResultAttribute<T>
        {
            public T LeftHandValue ;
            public T RightHandValue ;
        }

        public ResultAttribute<float> GetCriticalDamage(Character c, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;
            var dmg = GetDamageByWeaponType(c, takedDamageWeaponType);
            var critMod = GetTotalCriticalMod(c.CommonProfile, takedDamageWeaponType);
            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = dmg.LeftHandValue + dmg.LeftHandValue * critMod.LeftHandValue * c.CommonProfile.Characteristics.Speed.Strenght / 100f;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = dmg.RightHandValue + dmg.RightHandValue * critMod.RightHandValue * c.CommonProfile.Characteristics.Speed.Strenght / 100f;
                }
            }
            return result;
        }

        public ResultAttribute<float> GetBaseDamage(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.BaseDamage;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.BaseDamage;
                }
            }
            return result;
        }

        public ResultAttribute<float> GetSharpening(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.Sharpening;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.Sharpening;
                }
            }
            return result;
        }

        public ResultAttribute<EnergyTypes> GetEnergyDamage(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<EnergyTypes>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.EnergyTypes;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.EnergyTypes;
                }
            }
            return result;
        }

        public ResultAttribute<float> GetAttackArea(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.AttackArea;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.AttackArea;
                }
            }
            return result;
        }

        public ResultAttribute<float> GetMaxTargets(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            var m = ModulesTypesOfImpacts.MeleeMaxTargets;
            switch (takedDamageWeaponType)
            {
                case WeaponTypes.Magic:
                    m = ModulesTypesOfImpacts.MagicMaxTargets;
                    break;
            }

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.MaxTargets * cp.GetModuleMultiplier(m);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.MaxTargets * cp.GetModuleMultiplier(m);
                }
            }

            return result;
        }

        public ResultAttribute<float> GetDisruptionMod(CommonCharacterProfile.CommonCharacterProfile cp ,WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            var m = ModulesTypesOfImpacts.MeleeDisruptionMod;
            switch (takedDamageWeaponType)
            {
                case WeaponTypes.Ranged:
                    m = ModulesTypesOfImpacts.RangeDisruptionMod;
                    break;
                case WeaponTypes.Magic:
                    m = ModulesTypesOfImpacts.MagicDisruptionMod;
                    break;
            }

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.DisruptionModifier * cp.GetModuleMultiplier(m);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.DisruptionModifier * cp.GetModuleMultiplier(m);
                }
            }

            return result;
        }



        public ResultAttribute<SpecialAttackTypes> GetSpecialAttack(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<SpecialAttackTypes>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.SpecialAttack;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.SpecialAttack;
                }
            }

            return result;
        }

        public ResultAttribute<RaretyTypes> GetRarityForPlace(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<RaretyTypes>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.Rarety;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.Rarety;
                }
            }
            return result;
        }

        public ResultAttribute<float> GetWeightFor(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.Weight;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.Weight;
                }
            }
            return result;
        }

        public ResultAttribute<float> GetAnomality(Character c, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.GetTotalAnomality(c);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.GetTotalAnomality(c);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetAttackSpeed(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            var m = ModulesTypesOfImpacts.MeleeAttackSpeed;
            switch (takedDamageWeaponType)
            {
                case WeaponTypes.Ranged:
                    m = ModulesTypesOfImpacts.RangeAttackSpeed;
                    break;
                case WeaponTypes.Magic:
                    m = ModulesTypesOfImpacts.MagicAttackSpeed;
                    break;
            }

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.AttackSpeed * cp.GetModuleMultiplier(m);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.AttackSpeed * cp.GetModuleMultiplier(m);
                }
            }

            return result;
        }

        public ResultAttribute<float> GetChargeRate(Character c, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;
            
            var attackSpeed = GetAttackSpeed(c.CommonProfile, takedDamageWeaponType);
            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = c.CommonProfile.Mastery.Body.Vision.Value / 20f / attackSpeed.LeftHandValue * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MagicChargeVelocity);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = c.CommonProfile.Mastery.Body.Vision.Value / 20f / attackSpeed.RightHandValue * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MagicChargeVelocity);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetAtackDistance(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.AttackRange * cp.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeAttackDistance);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.AttackRange * cp.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeAttackDistance);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetDamageRadius(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.DamageRadius * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeDamageRadius);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.DamageRadius * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeDamageRadius);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetAttackRange(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.AttackRange* cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeAttackRange); 
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.AttackRange * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeAttackRange);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetAccuracy(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.Accuracy * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeAccuracy);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.Accuracy * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeAccuracy);
                }
            }

            return result;
        }

        public ResultAttribute<float> GetShotInLine(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.ShotsInLine * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeShotInLine);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.ShotsInLine * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeShotInLine);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetCriticalMod(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            var m = ModulesTypesOfImpacts.MeleeCriticalMod;
            switch (takedDamageWeaponType)
            {
                case WeaponTypes.Ranged:
                    m = ModulesTypesOfImpacts.RangeCriticalMod;
                    break;
                case WeaponTypes.Magic:
                    m = ModulesTypesOfImpacts.MagicCriticalMod;
                    break;
            }

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.CriticalModifier * cp.GetModuleMultiplier(m);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.CriticalModifier * cp.GetModuleMultiplier(m);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetStrength(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.Strength;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.Strength;
                }
            }
            return result;
        }

        public ResultAttribute<float> GetHolder(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.Holder * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeHolder);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.Holder * cp.GetModuleMultiplier(ModulesTypesOfImpacts.RangeHolder);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetCharger(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.Charger * cp.GetModuleMultiplier(ModulesTypesOfImpacts.MagicCharger);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.Charger * cp.GetModuleMultiplier(ModulesTypesOfImpacts.MagicCharger);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetTotalCriticalMod(CommonCharacterProfile.CommonCharacterProfile cp, WeaponTypes takedDamageWeaponType)
        {
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            var m = ModulesTypesOfImpacts.MeleeCriticalMod;
            switch (takedDamageWeaponType)
            {
                case WeaponTypes.Ranged:
                    m = ModulesTypesOfImpacts.RangeCriticalMod;
                    break;
                case WeaponTypes.Magic:
                    m = ModulesTypesOfImpacts.MeleeCriticalMod;
                    break;
            }

            var result = new ResultAttribute<float>();
            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)

                {
                    result.LeftHandValue = weaponInLeftHand.CriticalModifier * cp.GetModuleMultiplier(m);
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)

                {
                    result.RightHandValue = weaponInRightHand.CriticalModifier * cp.GetModuleMultiplier(m);
                }
            }
            return result;
        }

        public ResultAttribute<float> GetCriticalShot(WeaponTypes takedDamageWeaponType)
        {
            var result = new ResultAttribute<float>();
            var weaponInLeftHand = LeftWeapon;
            var weaponInRightHand = RightWeapon;

            if (weaponInLeftHand != null)
            {
                if (weaponInLeftHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.LeftHandValue = weaponInLeftHand.CriticalShot;
                }
            }
            if (weaponInRightHand != null)
            {
                if (weaponInRightHand.WeaponType == takedDamageWeaponType || takedDamageWeaponType == WeaponTypes.None)
                {
                    result.RightHandValue = weaponInRightHand.CriticalShot;
                }
            }
            return result;
        }

        public float GetModulesCapacity()
        {
            var armor = Armor;
            var result = 0;
            if (armor == null) return result;
            for (var i = 0; i < armor.ArmorParts.Count; ++i)
            {
                for (var j = 0; j < armor.ArmorParts[i].Modules.Count; j++)
                {
                    result += armor.ArmorParts[i].Modules[j].ModuleCapacity;
                }
            }

            return result;
        }
        #endregion
    }
}