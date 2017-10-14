
// ReSharper disable MergeConditionalExpression
// ReSharper disable ForCanBeConvertedToForeach

namespace YourGame.Common.Domain.CommonCharacterProfile
{
    using System;
    using System.Collections.Generic;

    using YourGame.Common.Domain.CommonCharacterProfile.Groups;
    using YourGame.Common.Domain.CommonCharacterProfile.Hidden;
    using YourGame.Common.Domain.Enums;

    public class CommonCharacterProfile
    {
        public static readonly int[] CommonAbv = { 195, 195, 234, 280, 336, 404, 485, 581, 698, 837, 1005, 1206, 1447, 1736, 2083, 2500 };
        #region Fields

        private Character _character;
        private readonly Dictionary<ModulesTypesOfImpacts, int> _percentModulesDict = new Dictionary<ModulesTypesOfImpacts, int>();
        public int CombatPotential;

        public int Experience;
        public int Level;

        public int WillpowerBossPoints;
        public int ImprovmentsBossPoints;
        public int AdaptabilityBossPoints;
        public int EnduranceBossPoints;

        public OffenceGroup Offence;
        public DefenceGroup Defence;
        public MasteryGroup Mastery;
        public CharacteristicsGroup Characteristics;
        public HiddenGroup Hidden;

        public CombatProfile Combat;
        #endregion

        #region Ctor

        public CommonCharacterProfile()
        {
            Offence = new OffenceGroup();
            Defence = new DefenceGroup();
            Mastery = new MasteryGroup();
            Hidden = new HiddenGroup();
            Characteristics = new CharacteristicsGroup();
            Combat = new CombatProfile();
        }

        public void Init(Character c)
        {
            _character = c;
        }
        #endregion

        #region Methods

        public void Calculate()
        {
            Experience = _character.Experience;
            Level = _character.Level;

            var levelMod = DomainConfiguration.EquipmentLevelPercents[Level];
            var armor = _character.Equipment.Armor;

            // Заполняем процентовку модулей
            if (armor?.ArmorParts != null)
            {
                for (var i = 0; i < armor.ArmorParts.Count; i++)
                {
                    var ap = armor.ArmorParts[i];
                    if (ap.Modules == null || ap.Modules.Count == 0) continue;
                    for (var j = 0; j < ap.Modules.Count; j++)
                    {
                        var m = ap.Modules[j];
                        if (_percentModulesDict.ContainsKey(m.TypesOfImpact))
                        {
                            _percentModulesDict[m.TypesOfImpact] += m.Value;
                        }
                        else
                        {
                            _percentModulesDict.Add(m.TypesOfImpact, m.Value);
                        }
                    }
                }
            }

            // Расчитываем характеристики
            Characteristics.Calculate(_character);

            // Заполняем вторички второго и первого порядка и что еще там известно
            Mastery.Calculate(_character);
            Offence.Calculate(_character);
            Defence.Calculate(_character);


            // считаем первички
            var primaryAttack = GetCharacteristicSum(Characteristics.Speed.Value, Characteristics.Power.Value, Characteristics.Intellect.Value);
            var primaryDefence = GetCharacteristicSum(Characteristics.Dexterity.Value, Characteristics.Wisdom.Value, Characteristics.Endurance.Value);
            var primaryAccuracy = GetCharacteristicSum(Characteristics.Intellect.Value, Characteristics.Speed.Value, Characteristics.Wisdom.Value);
            var primaryImmunity = GetCharacteristicSum(Characteristics.Endurance.Value, Characteristics.Dexterity.Value, Characteristics.Power.Value);
            var primaryPenetration = GetCharacteristicSum(Characteristics.Power.Value, Characteristics.Speed.Value, Characteristics.Endurance.Value);
            var primaryResistance = GetCharacteristicSum(Characteristics.Wisdom.Value, Characteristics.Dexterity.Value, Characteristics.Intellect.Value);
            var primaryMind = GetCharacteristicSum(Characteristics.Intellect.Value, Characteristics.Wisdom.Value, Characteristics.Dexterity.Value);
            var primaryMovement = GetCharacteristicSum(Characteristics.Endurance.Value, Characteristics.Power.Value, Characteristics.Speed.Value);

            var melleAttackMod = GetAtackMod(WeaponTypes.Melee);
            var rangeAttackMod = GetAtackMod(WeaponTypes.Ranged);
            var magicAttackMod = GetAtackMod(WeaponTypes.Magic);

            var melleDefMod = GetDefMod(WeaponTypes.Melee);
            var rangeDefMod = GetDefMod(WeaponTypes.Ranged);
            var magicDefMod = GetDefMod(WeaponTypes.Magic);

            var melleAttack = primaryAttack / 2 + primaryDefence / 4 + 5;
            melleAttack += melleAttack * melleAttackMod;
            Offence.MeleeAttack.Value.Value = melleAttack;

            var rangeAttack = primaryAccuracy / 2 + primaryImmunity / 4 + 5;
            rangeAttack += rangeAttack * rangeAttackMod;
            Offence.RangedAttack.Value.Value = rangeAttack;

            var magicAttack = primaryPenetration / 2 + primaryResistance / 4 + 5;
            magicAttack += magicAttack * magicAttackMod;
            Offence.MagicAttack.Value.Value = magicAttack;

            var melleDef = primaryDefence / 2 + primaryAttack / 4 + 5;
            melleDef += melleDef * melleDefMod;
            Defence.MeleeDefense.Value.Value = melleDef;

            var rangeDef = primaryImmunity / 2 + primaryAccuracy / 4 + 5;
            rangeDef += rangeDef * rangeDefMod;
            Defence.RangedDefense.Value.Value = rangeDef;

            var magicDef = primaryResistance / 2 + primaryPenetration / 4 + 5;
            magicDef += magicDef * magicDefMod;
            Defence.MagicDefense.Value.Value = magicDef;

            Mastery.Mind.Value.Value = primaryMind / 2 + primaryMovement / 4 + 5;

            Mastery.Body.Value.Value = primaryMovement / 2 + primaryMind / 4 + 5;

            Mastery.Skill.Value.Value = (Characteristics.Speed.Percent + Characteristics.Power.Percent + Characteristics.Intellect.Percent +
                                         Characteristics.Wisdom.Percent + Characteristics.Endurance.Percent + Characteristics.Dexterity.Percent) /
                                            3.5f + 5;

            // считаем % первичек

            var wRh = _character.Equipment.RightWeapon;
            var wLh = _character.Equipment.LeftWeapon;

            // учитываем мод комбинаций 
            var combinationMod = 1f;
            if (wRh != null && wLh != null)
            {
                if (wRh.WeaponType == wLh.WeaponType && wLh.WeaponSize != WeaponSizeTypes.Shield && wRh.WeaponType != WeaponTypes.Magic)
                {
                    combinationMod = 1.5f;
                }
            }
            if ((wRh != null && wRh.WeaponType == WeaponTypes.Melee && (wLh == null || wLh.WeaponSize == WeaponSizeTypes.Shield)) ||
                (wRh != null && wLh != null && wRh.WeaponType == WeaponTypes.Melee && wLh.WeaponType == WeaponTypes.Melee)) combinationMod *= 2f;


            // Damage potential
            var dps = Math.Max(Offence.MeleeAttack.DamagePotential.Value1, Offence.MeleeAttack.DamagePotential.Value2);
            var oneWeapon = Offence.MeleeAttack.DamagePotential.Value1 >= Offence.MeleeAttack.DamagePotential.Value2 ? wRh : (wLh != null ? wLh : wRh);
            Offence.MeleeAttack.DamagePotential.BpPercent = (oneWeapon != null) ? dps / 25f * oneWeapon.GetRaretyModifier(_character.Level, true) * combinationMod : 0;

            dps = Math.Max(Offence.RangedAttack.DamagePotential.Value1, Offence.RangedAttack.DamagePotential.Value2);
            oneWeapon = Offence.RangedAttack.DamagePotential.Value1 >= Offence.RangedAttack.DamagePotential.Value2 ? wRh : (wLh != null ? wLh : wRh);
            Offence.RangedAttack.DamagePotential.BpPercent = (oneWeapon != null) ? dps / 8f * oneWeapon.GetRaretyModifier(_character.Level, true) * combinationMod : 0;

            dps = Math.Max(Offence.MagicAttack.DamagePotential.Value1, Offence.MagicAttack.DamagePotential.Value2);
            oneWeapon = Offence.MagicAttack.DamagePotential.Value1 >= Offence.MagicAttack.DamagePotential.Value2 ? wRh : (wLh != null ? wLh : wRh);
            Offence.MagicAttack.DamagePotential.BpPercent = (oneWeapon != null) ? dps / 6f * oneWeapon.GetRaretyModifier(_character.Level, true) * combinationMod : 0;

            // Помехи
            Offence.MeleeAttack.AbilityBreak.BpPercent = Offence.MeleeAttack.AbilityBreak.Value / 60f * combinationMod;
            Offence.RangedAttack.MovementBreak.BpPercent = Offence.RangedAttack.MovementBreak.Value / 60f * combinationMod;
            Offence.MagicAttack.SpiritBreak.BpPercent = Offence.MagicAttack.SpiritBreak.Value / 60f * combinationMod;

            // Сопротивления
            Defence.MeleeDefense.AbilityResist.BpPercent = Defence.MeleeDefense.AbilityResist.Value / 90f;
            Defence.RangedDefense.MovementResist.BpPercent = Defence.RangedDefense.MovementResist.Value / 90f;
            Defence.MagicDefense.SpiritResist.BpPercent = Defence.MagicDefense.SpiritResist.Value / 90f;


            var armorMod = armor == null ? 1 : (armor.ArmorType == ArmorTypes.Heavy ? 1 : (armor.ArmorType == ArmorTypes.Medium ? 1.3f : 1.7f));
            // Бронька
            Defence.MeleeDefense.FireShield.Value = armor == null ? 0 : armor.FireShield / 7.8f * levelMod;
            Defence.MeleeDefense.IceShield.Value = armor == null ? 0 : armor.IceShield / 7.8f * levelMod;
            Defence.MeleeDefense.ElectricityShield.Value = armor == null ? 0 : armor.ElectricityShield / 7.8f * levelMod;

            Defence.MeleeDefense.TotalDurability.BpPercent = (Defence.MeleeDefense.TotalDurability.Value / 100 * 1.3f * armorMod) *
                                                             (1 + (Defence.MeleeDefense.Armor.Value + Defence.RangedDefense.Armor.Value + Defence.MagicDefense.Armor.Value) / 300f);

            // Проценты по разделам
            Offence.MeleeAttack.Value.BpPercent = Offence.MeleeAttack.DamagePotential.BpPercent +
                                                  Offence.MeleeAttack.AbilityBreak.BpPercent;
            Offence.RangedAttack.Value.BpPercent = Offence.RangedAttack.DamagePotential.BpPercent +
                                                  Offence.RangedAttack.MovementBreak.BpPercent;
            Offence.MagicAttack.Value.BpPercent = Offence.MagicAttack.DamagePotential.BpPercent +
                                                  Offence.MagicAttack.SpiritBreak.BpPercent;
            Defence.MeleeDefense.Value.BpPercent = Defence.MeleeDefense.TotalDurability.BpPercent +
                                                   Defence.MeleeDefense.AbilityResist.BpPercent;
            Defence.RangedDefense.Value.BpPercent = Defence.RangedDefense.MovementResist.BpPercent;
            Defence.MagicDefense.Value.BpPercent = Defence.MagicDefense.SpiritResist.BpPercent;
            Mastery.Mind.Value.BpPercent = Mastery.Mind.Willpower.BpPercent;
            Mastery.Body.Value.BpPercent = Mastery.Body.Improvements.BpPercent;
            Mastery.Skill.Value.BpPercent = Mastery.Skill.Autonomy.BpPercent + Mastery.Skill.Adaptability.BpPercent;

            // Считаем бп разделов
            // Offence
            var baseValue = Offence.RangedAttack.Value.Value + Offence.MeleeAttack.Value.Value + Offence.MagicAttack.Value.Value;
            var modulesPercent = 0;
            var basePercent = 1 + (Offence.RangedAttack.Value.BpPercent + Offence.MeleeAttack.Value.BpPercent +
                              Offence.MagicAttack.Value.BpPercent) / 100f + modulesPercent;
            Offence.Summary = (int)Math.Round(baseValue * basePercent);

            // Defence
            baseValue = Defence.MeleeDefense.Value.Value + Defence.RangedDefense.Value.Value + Defence.MagicDefense.Value.Value;
            modulesPercent = 0;
            basePercent = 1 + (Defence.MeleeDefense.Value.BpPercent + Defence.RangedDefense.Value.BpPercent +
                              Defence.MagicDefense.Value.BpPercent) / 100f + modulesPercent;
            Defence.Summary = (int)Math.Round(baseValue * basePercent);

            // Mastery
            baseValue = Mastery.Body.Value.Value + Mastery.Mind.Value.Value + Mastery.Skill.Value.Value;
            modulesPercent = 0;
            basePercent = 1 + (Mastery.Body.Value.BpPercent + Mastery.Mind.Value.BpPercent +
                              Mastery.Skill.Value.BpPercent) / 100f + modulesPercent;
            Mastery.Summary = (int)Math.Round(baseValue * basePercent);

            // БП
            CombatPotential = Offence.Summary + Defence.Summary + Mastery.Summary;

            // Считаем скрытые
            Hidden.Reversal = Characteristics.Dexterity.Strenght / 100f;
            Hidden.ReversalChance = Characteristics.Dexterity.Percent;

            var a1 = Offence.MeleeAttack.CriticalMod.Value1 > 0 ? Offence.MeleeAttack.CriticalMod.Value1 : float.MaxValue;
            var a2 = Offence.MeleeAttack.CriticalMod.Value2 > 0 ? Offence.MeleeAttack.CriticalMod.Value2 : float.MaxValue;
            var critModMelee = Math.Min(a1, a2);

            var r1 = Offence.RangedAttack.CriticalMod.Value1 > 0 ? Offence.RangedAttack.CriticalMod.Value1 : float.MaxValue;
            var r2 = Offence.RangedAttack.CriticalMod.Value2 > 0 ? Offence.RangedAttack.CriticalMod.Value2 : float.MaxValue;
            var r3 = Offence.MagicAttack.CriticalMod.Value1 > 0 ? Offence.MagicAttack.CriticalMod.Value1 : float.MaxValue;
            var r4 = Offence.MagicAttack.CriticalMod.Value2 > 0 ? Offence.MagicAttack.CriticalMod.Value2 : float.MaxValue;
            var critModRanged = Math.Min(Math.Min(r1, r2), Math.Min(r3, r4));

            Hidden.MeleeCritStrength = critModMelee * Characteristics.Speed.Strenght / 100;
            Hidden.RangedCritStrength = critModRanged * Characteristics.Speed.Strenght / 100;


            ApplyModulesPercents(_percentModulesDict);

            Combat.Reset(_character);
        }

        public float GetModuleMultiplier(ModulesTypesOfImpacts target)
        {
            return 1f + GetSeconadryModulesModifer(target);
        }

        public int GetSeconadryModulesModifer(ModulesTypesOfImpacts target)
        {
            return _percentModulesDict.ContainsKey(target) ? _percentModulesDict[target] : 0;
        }

        private void ApplyModulesPercents(Dictionary<ModulesTypesOfImpacts, int> percentModulesDict)
        {
            percentModulesDict.Clear();
            foreach (var target in percentModulesDict)
            {
                switch (target.Key)
                {
                    case ModulesTypesOfImpacts.MeleeAttackSpeed:
                        Offence.MeleeAttack.AttackSpeed.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeAttackDistance:
                        Offence.MeleeAttack.AttackDistance.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeMaxTargets:
                        Offence.MeleeAttack.MaxTargets.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeDisruptionMod:
                        Offence.MeleeAttack.DisruptionMod.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeCriticalMod:
                        Offence.MeleeAttack.CriticalMod.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeWeight:
                        Offence.MeleeAttack.Weight.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeAnomality:
                        Offence.MeleeAttack.Anomaly.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeAttackSpeed:
                        Offence.RangedAttack.AttackSpeed.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeDamageRadius:
                        Offence.RangedAttack.DamageRadius.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeAttackRange:
                        Offence.RangedAttack.AttackRange.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeDisruptionMod:
                        Offence.RangedAttack.DisruptionMod.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeCriticalMod:
                        Offence.RangedAttack.CriticalMod.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeWeight:
                        Offence.RangedAttack.Weight.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeAnomality:
                        Offence.RangedAttack.Anomaly.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeAccuracy:
                        Offence.RangedAttack.Accuracy.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeShotInLine:
                        Offence.RangedAttack.ShotInLine.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeHolder:
                        Offence.RangedAttack.Holder.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicAttackSpeed:
                        Offence.MagicAttack.AttackSpeed.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicMaxTargets:
                        Offence.MagicAttack.MaxTargets.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicChargeVelocity:
                        Offence.MagicAttack.ChargeRate.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicDisruptionMod:
                        Offence.MagicAttack.DisruptionMod.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicCriticalMod:
                        Offence.MagicAttack.CriticalMod.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicCharger:
                        Offence.MagicAttack.Charger.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicWeight:
                        Offence.MagicAttack.Weight.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicAnomality:
                        Offence.MagicAttack.Anomaly.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeDefenceAbilityResist:
                        Defence.MeleeDefense.AbilityResist.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeDefenceAttackAngle:
                        Defence.MeleeDefense.AttackAngle.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MeleeDefenceGeneralCasing:
                        Defence.MeleeDefense.GeneralCasing.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.RangeDefenceMovementResist:
                        Defence.RangedDefense.MovementResist.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MagicDefenceSpiritResist:
                        Defence.MagicDefense.SpiritResist.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MindImmunity:
                        Mastery.Mind.Immunity.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MindInitiative:
                        Mastery.Mind.Initiative.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.MindStamina:
                        Mastery.Mind.Stamina.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.BodyMovement:
                        Mastery.Body.Movement.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.BodyCharge:
                        Mastery.Body.Charge.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.BodyVision:
                        Mastery.Body.Vision.BpPercent = target.Value;
                        break;
                    case ModulesTypesOfImpacts.PowerValue:
                        break;
                    case ModulesTypesOfImpacts.SpeedValue:
                        break;
                    case ModulesTypesOfImpacts.IntellectValue:
                        break;
                    case ModulesTypesOfImpacts.EnduranceValue:
                        break;
                    case ModulesTypesOfImpacts.DexteretyValue:
                        break;
                    case ModulesTypesOfImpacts.WisdomValue:
                        break;
                }
            }
        }

        private float GetDefMod(WeaponTypes targetType)
        {
            var wRh = _character.Equipment.RightWeapon;
            var wLh = _character.Equipment.LeftWeapon;

            // в правой экзотика и щит в левой
            if (wRh != null && wRh.WeaponSize == WeaponSizeTypes.Small && wLh != null && wLh.WeaponSize == WeaponSizeTypes.Shield && wLh.WeaponType == targetType) return 0.2f;

            // В правой обычное в левой щит
            if (wRh != null && wRh.WeaponSize == WeaponSizeTypes.None && wLh != null && wLh.WeaponSize == WeaponSizeTypes.Shield && wLh.WeaponType == targetType) return 0.1f;

            return 0;
        }

        private float GetAtackMod(WeaponTypes targetType)
        {
            var wRh = _character.Equipment.RightWeapon;
            var wLh = _character.Equipment.LeftWeapon;

            // В руках двуручная экзотика (в правой, во второй должно быть пусто) 
            if (wRh != null && wRh.WeaponType == targetType && wRh.WeaponSize == WeaponSizeTypes.Small && wRh.WeaponHands == WeaponHandsTypes.TwoHand) return 0.1f;

            // в руках только одноручная экзотика
            if (wRh != null && wRh.WeaponType == targetType && wRh.WeaponSize == WeaponSizeTypes.Small && wRh.WeaponHands == WeaponHandsTypes.OneHand && wLh == null) return 0.2f;

            // в руках только одноручное обычное
            if (wRh != null && wRh.WeaponType == targetType && wRh.WeaponSize == WeaponSizeTypes.None && wRh.WeaponHands == WeaponHandsTypes.OneHand && wLh == null) return 0.1f;
            //===========================================================================================================================

            // в правой экзотика и щит в левой
            if (wRh != null && wRh.WeaponType == targetType && wRh.WeaponSize == WeaponSizeTypes.Small && wLh != null && wLh.WeaponSize == WeaponSizeTypes.Shield) return 0.1f;

            //===========================================================================================================================
            // в одной экзотика и экзотика в другой, однотипные
            if (wRh != null && wLh != null && (wRh.WeaponType == targetType && wLh.WeaponType == targetType) &&
                (wRh.WeaponSize == WeaponSizeTypes.Small && wLh.WeaponSize == WeaponSizeTypes.Small)) return 0.1f;

            // в одной обычное и обычное в другой, однотипные
            if (wRh != null && wLh != null && wRh.WeaponType == targetType && wLh.WeaponType == targetType &&
                wRh.WeaponSize == WeaponSizeTypes.None && wLh.WeaponSize == WeaponSizeTypes.None) return -0.1f;

            //===========================================================================================================================
            // В обеих руках обычное оружие разнотипное
            if (wRh != null && wLh != null && wRh.WeaponSize == WeaponSizeTypes.None && wLh.WeaponSize == WeaponSizeTypes.None &&
                (wRh.WeaponType == targetType || wLh.WeaponType == targetType) && wRh.WeaponType != wLh.WeaponType) return -0.2f;
            // В обеих руках экзотика разнотипное
            if (wRh != null && wLh != null && wRh.WeaponSize == WeaponSizeTypes.Small && wLh.WeaponSize == WeaponSizeTypes.Small &&
                (wRh.WeaponType == targetType || wLh.WeaponType == targetType) && wRh.WeaponType != wLh.WeaponType) return -0.1f;

            return 0;
        }

        private static readonly List<Dictionary<int, float>> CharacteristicsDesc = new List<Dictionary<int, float>>
        {
                new Dictionary<int, float>{
                    { 1, 0},
                    {2, 5},
                    {3, 10},
                    {4, 15},
                    {5, 22.5f},
                    {6, 30},
                    {7, 45}
                },

                new Dictionary<int, float>{
                    {1, 0},
                    {2, 4},
                    {3, 8},
                    {4, 12},
                    {5, 18},
                    {6, 24},
                    {7, 36}
                },

                new Dictionary<int, float>{
                    {1, 0},
                    {2, 3},
                    {3, 6},
                    {4, 9},
                    {5, 13.5f},
                    {6, 18},
                    {7, 27}
                }
        };

        private static float GetCharacteristicSum(int c1, int c2, int c3)
        {
            var v1 = CharacteristicsDesc[0][c1];
            var v2 = CharacteristicsDesc[1][c2];
            var v3 = CharacteristicsDesc[2][c3];
            return v1 + v2 + v3;
        }
        #endregion

        #region Battle functional

        public void BattleArmorPartDestroy(ArmorPartTypes armorType)
        {
            Combat.Reset(_character);
        }

        public void BattleArmorPartRestore(ArmorPartTypes armorType)
        {
            Combat.Reset(_character);
        }

        public void BattleFullRestoreCombatProfile()
        {
            Combat.Reset(_character);
        }
        #endregion
    }
}