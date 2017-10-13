using System;
using Warsmiths.Common.Domain.CommonCharacterProfile.Attributes;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Attack
{
    public class MagicAttackPrimary
    {
        public Atribute<float> Value;

        // Вторички
        public Atribute2<float> DamagePotential;
        public Atribute<float> SpiritBreak;

        // Вторички 2 порядка
        public Atribute2<float> MeleeDamage;
        public Atribute2<float> CriticalDamage;
        public Atribute2<float> BaseDamage;
        public Atribute<float> MagicDamage;
        public Atribute<float> CriticalBlast;
        public Atribute2<float> Sharpening;
        public Atribute2<EnergyTypes> EnergyDamage;
        public Atribute<float> AttackSpeed;
        public Atribute<float> MaxTargets;
        public Atribute<float> ChargeRate;
        public Atribute<float> DisruptionMod;
        public Atribute2<float> CriticalMod;
        public Atribute<float> Charger;
        public Atribute2<SpecialAttackTypes> SpecialAttack;
        public Atribute2<RaretyTypes> Rarity;
        public Atribute2<float> Weight;
        public Atribute2<float> Anomaly;

        public MagicAttackPrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Primary,
                Name = "Magic Attack",
                Description = ""
            };
            //----------------------------------------------
            DamagePotential = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary1,
                Value1 = 0,
                Value2 = 0,
                Name = "Damage Potential",
                Description = "Средний урон от оружия с учетом всех модификаторов в реальном бою"
            };
            SpiritBreak = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary1,
                Value = 0,
                Name = "Spirit Break",
                Description = "Способность снизить атакующий и защитный потенциал противника"
            };
            //----------------------------------------------
            MeleeDamage = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Damage",
                Description = "Урон с учетом базового урона, редкости и заточки оружия, повышается с уровнем"
            };
            CriticalDamage = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Critical Damage",
                Description = "Зависит от критического потенциала оружия и силы навыка Скорости"
            };
            BaseDamage = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Base Damage",
                Description = "Урон конкретного вида оружия с учетом качества"
            };
            MagicDamage = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value = 0,
                Name = "Blast Damage",
                Description = "Урон одного кинетического разряда жезла"
            };
            CriticalBlast = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value = 0,
                Name = "Critical Blast",
                Description = "Зависит от критического потенциала оружия и силы навыка Скорости"
            };
            Sharpening = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Sharpening",
                Description = "Ультимативный способ повысить урон оружия свыше его редкости"
            };
            EnergyDamage = new Atribute2<EnergyTypes>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = EnergyTypes.None,
                Value2 = EnergyTypes.None,
                Name = "Energy Damage",
                Description = "Один из трех типов урона на оружии: огонь, лед, электричество"
            };
            AttackSpeed = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value = 0,
                Name = "Attack Speed",
                Description = "Скорость удара  в ближнем бою, влияет на скорость заряда с учетом обзора"
            };
            MaxTargets = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value = 0,
                Name = "Max Targets",
                Description = "Способность оружия поразить сразу с нескольких противников"
            };
            ChargeRate = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value = 0,
                Name = "Charge Rate",
                Description = "Скорость перемещения заряда магического оружия зависящая от обзора"
            };
            DisruptionMod = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value = 0,
                Name = "Disruption Mod.",
                Description = "Способность магического оружия делать противника уязвимым "
            };
            CriticalMod = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Critical Mod.",
                Description = "Максимальный потенциал увеличения урона при критическом попадании"
            };
            Charger = new Atribute<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value = 0,
                Name = "Charger",
                Description = "Количество зарядов до перезарядки длящейся 9 секунд"
            };
            SpecialAttack = new Atribute2<SpecialAttackTypes>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Special Attack",
                Description = "Способность тактического контроля оружия, активируемая абилкой 2 таера"
            };
            Rarity = new Atribute2<RaretyTypes>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = RaretyTypes.Regular,
                Value2 = RaretyTypes.Regular,
                Name = "Rarity",
                Description = "Начиная с редкого, каждая степень редкости дает +30% к базовому урону "
            };
            Weight = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Weight",
                Description = "Вес оружия повышает чардж и снижает скорость передвижения "
            };
            Anomaly = new Atribute2<float>
            {
                Type = SkillGroupTypes.Secondary2,
                Value1 = 0,
                Value2 = 0,
                Name = "Anomaly",
                Description = "Аномальность оружия повышает обзор и снижает стойкость"
            };
        }

        public void Calculate(Character c)
        {
            var wRh = c.Equipment.RightWeapon;
            var wLh = c.Equipment.LeftWeapon;
            var use2 = false;
            // ReSharper disable once InconsistentNaming
            var l2r = false;
            const WeaponTypes wType = WeaponTypes.Magic;

            if (wRh != null && wLh != null && wRh.WeaponType == wLh.WeaponType && wRh.WeaponType == wType && wLh.WeaponSize != WeaponSizeTypes.Shield)
            {
                use2 = true;
            }

            if (wRh != null && wLh != null && wRh.WeaponType != wType && wLh.WeaponType == wType && wLh.WeaponSize != WeaponSizeTypes.Shield)
            {
                l2r = true;
            }

            var damage = c.Equipment.GetMeleeDamage(c, wType);
            MeleeDamage.Value1 = l2r? damage.LeftHandValue : damage.RightHandValue;
            MeleeDamage.Value2 = use2 ? damage.LeftHandValue : 0;
            MeleeDamage.UseSecondValue = use2;

            var criticalDamage = c.Equipment.GetCriticalDamage(c, wType);
            CriticalDamage.Value1 = l2r? criticalDamage.LeftHandValue : criticalDamage.RightHandValue;
            CriticalDamage.Value2 = use2 ? criticalDamage.LeftHandValue : 0;
            CriticalDamage.UseSecondValue = use2;

            var baseDamage = c.Equipment.GetBaseDamage(wType);
            BaseDamage.Value1 = l2r? baseDamage.LeftHandValue : baseDamage.RightHandValue;
            BaseDamage.Value2 = use2 ? baseDamage.LeftHandValue : 0;
            BaseDamage.UseSecondValue = use2;

            var bDamage2 = c.Equipment.GetDamageByWeaponType(c, WeaponTypes.Magic);
            var bDamage = l2r ? bDamage2.LeftHandValue : bDamage2.RightHandValue;
            if (wRh != null && wLh != null)
            {
                if (wRh.WeaponType == WeaponTypes.Magic && wLh.WeaponType == WeaponTypes.Magic)
                {
                    bDamage = (bDamage2.RightHandValue + bDamage2.LeftHandValue)*0.75f;
                }
            }

            MagicDamage.Value = bDamage;
            // TODO: Calc
            CriticalBlast.Value = 0;
            var sharpening = c.Equipment.GetSharpening(wType);
            Sharpening.Value1 = l2r? sharpening.LeftHandValue : sharpening.RightHandValue;
            Sharpening.Value2 = use2 ? sharpening.LeftHandValue : 0;
            Sharpening.UseSecondValue = use2;

            var energyDamage = c.Equipment.GetEnergyDamage(wType);
            EnergyDamage.Value1 = l2r? energyDamage.LeftHandValue : energyDamage.RightHandValue;
            EnergyDamage.Value2 = use2 ? energyDamage.LeftHandValue : 0;
            EnergyDamage.UseSecondValue = use2;

            var attackSpeed = c.Equipment.GetAttackSpeed(c.CommonProfile, wType);
            var aSpeed1 = attackSpeed.RightHandValue > 0 ? attackSpeed.RightHandValue : 999999;
            var aSpeed2 = attackSpeed.LeftHandValue > 0 ? attackSpeed.LeftHandValue : 999999;
            var aSpeed = aSpeed1 < aSpeed2 ? aSpeed1 : aSpeed2;
            if (aSpeed >= 999999) aSpeed = 0;
            AttackSpeed.Value = aSpeed;

            MaxTargets.Value = c.Equipment.GetMaxTargets(c.CommonProfile, wType).RightHandValue;

            var chargeVelocity = c.Equipment.GetChargeRate(c, wType);
            ChargeRate.Value = Math.Max(chargeVelocity.RightHandValue, chargeVelocity.LeftHandValue);

            DisruptionMod.Value = c.Equipment.GetDisruptionMod(c.CommonProfile, wType).RightHandValue;

            var critMod = c.Equipment.GetCriticalMod(c.CommonProfile, wType);
            CriticalMod.Value1 = l2r? critMod.LeftHandValue : critMod.RightHandValue;
            CriticalMod.Value2 = use2 ? critMod.LeftHandValue : 0;
            CriticalMod.UseSecondValue = use2;

            // TODO: уточнить что это и зачем
            Charger.Value = c.Equipment.GetCharger(c.CommonProfile, wType).RightHandValue; 

            var specAttack = c.Equipment.GetSpecialAttack(wType);
            SpecialAttack.Value1 = specAttack.RightHandValue;
            SpecialAttack.Value2 = use2 ? specAttack.LeftHandValue : SpecialAttackTypes.None;
            SpecialAttack.UseSecondValue = use2;

            var rarity = c.Equipment.GetRarityForPlace(wType);
            Rarity.Value1 = l2r? rarity.LeftHandValue : rarity.RightHandValue;
            Rarity.Value2 = use2 ? rarity.LeftHandValue : 0;
            Rarity.UseSecondValue = use2;

            var weight = c.Equipment.GetWeightFor(wType);
            Weight.Value1 = l2r? weight.LeftHandValue : weight.RightHandValue;
            Weight.Value2 = use2 ? weight.LeftHandValue : 0;
            Weight.UseSecondValue = use2;

            var anomality = c.Equipment.GetAnomality(c, wType);
            Anomaly.Value1 = l2r? anomality.LeftHandValue : anomality.RightHandValue;
            Anomaly.Value2 = use2 ? anomality.LeftHandValue : 0;
            Anomaly.UseSecondValue = use2;

            DamagePotential.Value2 = 0;
            DamagePotential.UseSecondValue = false;
            DamagePotential.Value1 = 0;

            //ОДНОРУЧНОЕ В ПРАВОЙ или разнотипные и одно из них милишное
            if ((wRh != null && wLh == null && wRh.WeaponType == wType) 
                || (wRh != null && wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield && wRh.WeaponType != wLh.WeaponType && (wRh.WeaponType == wType || wLh.WeaponType == wType)))
            {
                var weapon = (wRh.WeaponType == wType) ? wRh : wLh;
                if (weapon != null)
                {
                    DamagePotential.Value2 = 0;
                    DamagePotential.UseSecondValue = false;

                    var a = MagicDamage.Value * (1f / weapon.AttackSpeed);
                    DamagePotential.Value1 = a * (1f + weapon.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                }
                else
                {
                    DamagePotential.Value2 = 0;
                    DamagePotential.UseSecondValue = false;
                    DamagePotential.Value1 = 0;
                }
            }
            //ДВУРУЧНОЕ В ПРАВОЙ
            else if (wRh != null && wRh.WeaponHands == WeaponHandsTypes.TwoHand && wRh.WeaponType == WeaponTypes.Magic)
            {
                var areaMod = new[] { 1, 1.3f, 1.6f };
                var a = MagicDamage.Value * (1f / wRh.AttackSpeed) * areaMod[wRh.MaxTargets - 1];
                DamagePotential.Value1 = a * (1f + wRh.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.UseSecondValue = false;
                DamagePotential.Value2 = 0;
            }
            // ДВА ОДНОТИПНЫХ
            else if (wRh != null && wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield && wLh.WeaponSize != WeaponSizeTypes.Shield && wRh.WeaponType == WeaponTypes.Magic && wRh.WeaponType == wLh.WeaponType)
            {
                var a1 = MagicDamage.Value * (1f / wRh.AttackSpeed);
                var a2 = MagicDamage.Value * (1f / wLh.AttackSpeed);

                DamagePotential.Value1 = a1 * (1f + wRh.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.Value2 = a2 * (1f + wRh.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.UseSecondValue = true;
            }
            // ДВА разнотипных 
            else if (wRh != null && wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield && (wRh.WeaponType == WeaponTypes.Magic || wLh.WeaponType == WeaponTypes.Magic) && wRh.WeaponType != wLh.WeaponType)
            {
                var weapon = (wRh.WeaponType == WeaponTypes.Magic) ? wRh : wLh;
                DamagePotential.Value2 = 0;
                DamagePotential.UseSecondValue = false;

                var a = MagicDamage.Value * (1f / weapon.AttackSpeed);
                DamagePotential.Value1 = a * (1f + weapon.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
            }

            // Помехи
            var abilityBreak1 = 99999999999f;
            var abilityBreak2 = 99999999999f;
            if (wRh != null && wRh.WeaponType == WeaponTypes.Magic)
            {
                abilityBreak1 = CommonCharacterProfile.CommonAbv[c.Level] * wRh.DisruptionModifier;
            }
            if (wLh != null && wLh.WeaponType == WeaponTypes.Magic)
            {
                abilityBreak2 = CommonCharacterProfile.CommonAbv[c.Level] * wLh.DisruptionModifier;
            }
            var abiBreak = Math.Min(abilityBreak1, abilityBreak2);
            if (abiBreak > 999999999f) abiBreak = 0;
            SpiritBreak.Value = abiBreak;
        }
    }
}
