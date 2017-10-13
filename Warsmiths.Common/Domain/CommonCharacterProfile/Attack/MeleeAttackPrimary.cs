using System;
using Warsmiths.Common.Domain.CommonCharacterProfile.Attributes;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Attack
{
    public class MeleeAttackPrimary
    {
        public Atribute<float> Value;

        // Вторички
        public Atribute2<float> DamagePotential;
        public Atribute<float> AbilityBreak;

        // Вторички 2 порядка
        public Atribute2<float> MeleeDamage;
        public Atribute2<float> CriticalDamage;
        public Atribute2<float> BaseDamage;
        public Atribute2<float> Sharpening;
        public Atribute2<EnergyTypes> EnergyDamage;
        public Atribute<float> AttackSpeed;
        public Atribute2<int> AttackArea;
        public Atribute<float> AttackDistance;
        public Atribute<int> MaxTargets;
        public Atribute2<float> DisruptionMod;
        public Atribute2<float> CriticalMod;
        public Atribute2<SpecialAttackTypes> SpecialAttack;
        public Atribute2<RaretyTypes> Rarity;
        public Atribute2<float> Weight;
        public Atribute2<float> Anomaly;

        public MeleeAttackPrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Name = "Melee Attack",
                Type = SkillGroupTypes.Primary,
                Description = ""
            };
            //---------------------------------------------------------
            DamagePotential = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Damage Potential",
                Type = SkillGroupTypes.Secondary1,
                Description = "Средний урон от оружия с учетом всех модификаторов в реальном бою"
            };

            AbilityBreak = new Atribute<float>
            {
                Value = 0,
                Name = "Ability Break",
                Type = SkillGroupTypes.Secondary1,
                Description = "Способность сбить врагу применение им его накопленного приема"
            };
            //---------------------------------------------------------
            MeleeDamage = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Damage",
                Type = SkillGroupTypes.Secondary2,
                Description = "Урон с учетом базового урона, редкости и заточки оружия, повышается с уровнем"
            };
            CriticalDamage = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Critical Damage",
                Type = SkillGroupTypes.Secondary2,
                Description = "Зависит от критического потенциала оружия и силы навыка Скорости"
            };
            BaseDamage = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Base Damage",
                Type = SkillGroupTypes.Secondary2,
                Description = "Урон конкретного вида оружия с учетом качества"
            };
            Sharpening = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Sharpening",
                Type = SkillGroupTypes.Secondary2,
                Description = "Ультимативный способ повысить урон оружия свыше его редкости"
            };
            EnergyDamage = new Atribute2<EnergyTypes>
            {
                Value1 = EnergyTypes.None,
                Value2 = EnergyTypes.None,
                Name = "Energy Damage",
                Type = SkillGroupTypes.Secondary2,
                Description = "Один из трех типов урона на оружии: огонь, лед, электричество"
            };
            AttackSpeed = new Atribute<float>
            {
                Value = 0,
                Name = "Attack Speed",
                Type = SkillGroupTypes.Secondary2,
                Description = "Скорость удара в секундах, чем медленнее, тем больше прирост энергий"
            };
            AttackArea = new Atribute2<int>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Attack Area",
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность оружия нанести урон сразу по нескольким частям доспеха"
            };
            AttackDistance = new Atribute<float>
            {
                Value = 0,
                Name = "Attack Distance",
                Type = SkillGroupTypes.Secondary2,
                Description = "Дальность удара ближним оружием "
            };
            MaxTargets = new Atribute<int>
            {
                Value = 0,
                Name = "Max Targets",
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность оружия сразиться сразу с несколькими противниками"
            };
            DisruptionMod = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Disruption Mod.",
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность ближнего оружия сбивать прием атакованному противнику"
            };
            CriticalMod = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Critical Mod.",
                Type = SkillGroupTypes.Secondary2,
                Description = "Максимальный потенциал увеличения урона при критическом ударе"
            };
            SpecialAttack = new Atribute2<SpecialAttackTypes>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Special Attack",
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность тактического контроля оружия, активируемая абилкой 2 таера"
            };
            Rarity = new Atribute2<RaretyTypes>
            {
                Value1 = RaretyTypes.Regular,
                Value2 = RaretyTypes.Regular,
                Name = "Rarity",
                Type = SkillGroupTypes.Secondary2,
                Description = "Начиная с редкого, каждая степень редкости дает 30% к базовому урону"
            };
            Weight = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Weight",
                Type = SkillGroupTypes.Secondary2,
                Description = "Вес оружия повышает чардж и снижает скорость передвижения"
            };
            Anomaly = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Anomaly",
                Type = SkillGroupTypes.Secondary2,
                Description = "Аномальность оружия повышает обзор и снижает стойкость"
            };
        }

        public void Calculate(Character c)
        {
            var wRh = c.Equipment.RightWeapon;
            var wLh = c.Equipment.LeftWeapon;
            var use2 = false;
            const WeaponTypes wType = WeaponTypes.None;

            if (wRh != null && wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield)
            {
                use2 = true;
            }

            // Вторички 2 порядка
            var damage = c.Equipment.GetMeleeDamage(c);
            var criticalDamage = c.Equipment.GetCriticalDamage(c, wType);
            var baseDamage = c.Equipment.GetBaseDamage(wType);
            var sharpening = c.Equipment.GetSharpening(wType);
            var energyDamage = c.Equipment.GetEnergyDamage(wType);
            var attackSpeed = c.Equipment.GetAttackSpeed(c.CommonProfile, wType);
            var attackArea = c.Equipment.GetAttackArea(wType);
            var attackDistance = c.Equipment.GetAtackDistance(c.CommonProfile, wType);
            var maxTargets = c.Equipment.GetMaxTargets(c.CommonProfile, wType);
            var disrMod = c.Equipment.GetDisruptionMod(c.CommonProfile, wType);
            var specialAttack = c.Equipment.GetSpecialAttack(wType);
            var rarity = c.Equipment.GetRarityForPlace(wType);
            var weight = c.Equipment.GetWeightFor(wType);
            var anomality = c.Equipment.GetAnomality(c, wType);
            var critMod = c.Equipment.GetTotalCriticalMod(c.CommonProfile, wType);

            MeleeDamage.Value1 =  damage.RightHandValue;
            MeleeDamage.Value2 = use2 ? damage.LeftHandValue : 0;
            MeleeDamage.UseSecondValue = use2;


            CriticalDamage.Value1 = criticalDamage.RightHandValue;
            CriticalDamage.Value2 = use2 ? criticalDamage.LeftHandValue : 0;
            CriticalDamage.UseSecondValue = use2;


            BaseDamage.Value1 = baseDamage.RightHandValue;
            BaseDamage.Value2 = use2 ? baseDamage.LeftHandValue : 0;
            BaseDamage.UseSecondValue = use2;


            Sharpening.Value1 = sharpening.RightHandValue;
            Sharpening.Value2 = use2 ? sharpening.LeftHandValue : 0;
            Sharpening.UseSecondValue = use2;
            
            EnergyDamage.Value1 = energyDamage.RightHandValue;
            EnergyDamage.Value2 = use2 ? energyDamage.LeftHandValue : EnergyTypes.None;
            EnergyDamage.UseSecondValue = use2;

            var aSpeed1 = attackSpeed.RightHandValue > 0 ? attackSpeed.RightHandValue : 999999;
            var aSpeed2 = attackSpeed.LeftHandValue > 0 ? attackSpeed.LeftHandValue : 999999;
            var aSpeed = aSpeed1 < aSpeed2 ? aSpeed1 : aSpeed2;
            if (aSpeed >= 999999) aSpeed = 0;
            AttackSpeed.Value = aSpeed;

            AttackArea.Value1 = (int)attackArea.RightHandValue;
            AttackArea.Value2 = use2 ? (int)attackArea.LeftHandValue : 0;
            AttackArea.UseSecondValue = use2;

            AttackDistance.Value = Math.Max(attackDistance.RightHandValue, attackDistance.LeftHandValue);


            if (use2)
            {
                MaxTargets.Value = (int)Math.Ceiling((maxTargets.RightHandValue + maxTargets.LeftHandValue)*0.75f);
            }
            else
            {
                MaxTargets.Value = (int)maxTargets.RightHandValue;
            }
            
            DisruptionMod.Value1 = disrMod.RightHandValue;
            DisruptionMod.Value2 = use2 ? disrMod.LeftHandValue : 0;
            DisruptionMod.UseSecondValue = use2;

            CriticalMod.Value1 = critMod.RightHandValue;
            CriticalMod.Value2 = use2 ? critMod.LeftHandValue  : 0;
            CriticalMod.UseSecondValue = use2;

            SpecialAttack.Value1 = specialAttack.RightHandValue;
            SpecialAttack.Value2 = use2 ? specialAttack.LeftHandValue : SpecialAttackTypes.None;
            SpecialAttack.UseSecondValue = use2;

            // TODO: use level rarity
            Rarity.Value1 = rarity.RightHandValue;
            Rarity.Value2 = use2 ? rarity.LeftHandValue : RaretyTypes.Rare;
            Rarity.UseSecondValue = use2;

            Weight.Value1 = weight.RightHandValue;
            Weight.Value2 = use2 ? weight.LeftHandValue : 0;
            Weight.UseSecondValue = use2;

            Anomaly.Value1 = anomality.RightHandValue;
            Anomaly.Value2 = use2? anomality.LeftHandValue : 0;
            Anomaly.UseSecondValue = use2;

            // Вторички
            DamagePotential.Value1 = 0;
            DamagePotential.Value2 = 0;
            DamagePotential.BpPercent = 0;
            AbilityBreak.Value = 0;
            AbilityBreak.BpPercent = 0;

            // Проверяем если у нас не милишные орудия в руках пишем их значения и константы... 
            if (wRh != null && wRh.WeaponType != WeaponTypes.Melee)
            {
                AttackArea.Value1 = 1;
                CriticalMod.Value1 = 0.5f * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeCriticalMod); 
                DisruptionMod.Value1 = 1f * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeDisruptionMod); 

                if (wRh.WeaponType == WeaponTypes.Ranged)
                {
                    MaxTargets.Value = wRh.WeaponHands == WeaponHandsTypes.OneHand ? 2 : 1;
                    var ad = wRh.WeaponHands == WeaponHandsTypes.OneHand ? 1.3f : 1.8f;
                    ad *= c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeAttackDistance);
                    AttackDistance.Value = ad;//AttackDistance.Value < ad ? ad : AttackDistance.Value;
                }
                else if (wRh.WeaponType == WeaponTypes.Magic)
                {
                    MaxTargets.Value = wRh.WeaponHands == WeaponHandsTypes.OneHand ? 4 : 2;
                    var ad = wRh.WeaponHands == WeaponHandsTypes.OneHand ? 1.5f : 2f;
                    ad *= c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeAttackDistance);
                    AttackDistance.Value = ad;//AttackDistance.Value < ad ? ad : AttackDistance.Value;
                }
            }

            if (wLh != null && wLh.WeaponType != WeaponTypes.Melee)
            {
                AttackArea.Value2 = 1;
                CriticalMod.Value2 = 0.5f * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeCriticalMod);
                DisruptionMod.Value2 = 1f * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeDisruptionMod);
                var mt = 0f;
                if (wLh.WeaponType == WeaponTypes.Ranged)
                {
                    mt = wLh.WeaponHands == WeaponHandsTypes.OneHand ? 2 : 1;
                    var ad = wLh.WeaponHands == WeaponHandsTypes.OneHand ? 1.3f : 1.8f;
                    ad *= c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeAttackDistance);
                    AttackDistance.Value = ad;//AttackDistance.Value < ad ? ad : AttackDistance.Value;
                }
                else if (wLh.WeaponType == WeaponTypes.Magic)
                {
                    mt = wLh.WeaponHands == WeaponHandsTypes.OneHand ? 4 : 2;
                    var ad = wLh.WeaponHands == WeaponHandsTypes.OneHand ? 1.5f : 2f;
                    ad *= c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeAttackDistance);
                    AttackDistance.Value = ad; // AttackDistance.Value < ad ? ad : AttackDistance.Value;
                }

                mt *= c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeMaxTargets);
                MaxTargets.Value = (int) Math.Ceiling((MaxTargets.Value + mt) *0.75f);
            }

            //ОДНОРУЧНОЕ В ПРАВОЙ 
            if (wRh != null && (wLh == null || wLh.WeaponSize == WeaponSizeTypes.Shield))
            {
                DamagePotential.Value2 = 0;
                DamagePotential.UseSecondValue = false;

                var a = wRh.GetMeleeDamage(c) * (AttackSpeed.Value);
                DamagePotential.Value1 = a * (1f + CriticalMod.Value1 * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                
            }

            //ДВУРУЧНОЕ В ПРАВОЙ
            else if (wRh != null && wRh.WeaponHands == WeaponHandsTypes.TwoHand)
            {
                var areaMod = new[] { 1, 1.3f, 1.6f };
                var a = wRh.GetMeleeDamage(c) * (1f / AttackSpeed.Value) * areaMod[AttackArea.Value1-1];
                DamagePotential.Value1 = a * (1f + CriticalMod.Value1 * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.UseSecondValue = false;
                DamagePotential.Value2 = 0;
            }

            // ДВА орудия
            else if (wRh != null)
            {
                var a1 = wRh.GetMeleeDamage(c) * (1f / wRh.AttackSpeed);
                var a2 = wLh.GetMeleeDamage(c) * (1f / wLh.AttackSpeed);

                DamagePotential.Value1 = a1 * (1f + CriticalMod.Value1 * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.Value2 = a2 * (1f + CriticalMod.Value2 * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.UseSecondValue = true;
            }
            
            // Помехи
            var abilityBreak1 = 99999999999f;
            var abilityBreak2 = 99999999999f;
            if (wRh != null)
            {
                abilityBreak1 = CommonCharacterProfile.CommonAbv[c.Level] * DisruptionMod.Value1;
            }
            if (wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield)
            {
                abilityBreak2 = CommonCharacterProfile.CommonAbv[c.Level] * DisruptionMod.Value2;
            }
            var abiBreak = Math.Min(abilityBreak1, abilityBreak2);
            if (abiBreak > 9999999999f) abiBreak = 0;
            AbilityBreak.Value = abiBreak;
        }
    }
}
