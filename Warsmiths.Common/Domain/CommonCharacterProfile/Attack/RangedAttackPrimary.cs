namespace YourGame.Common.Domain.CommonCharacterProfile.Attack
{
    using System;

    using YourGame.Common.Domain.CommonCharacterProfile.Attributes;
    using YourGame.Common.Domain.Enums;

    public class RangedAttackPrimary
    {
        public Atribute<float> Value;

        // Вторички
        public Atribute2<float> DamagePotential;
        public Atribute<float> MovementBreak;

        // Вторички 2 порядка
        public Atribute2<float> MeleeDamage;
        public Atribute2<float> CriticalDamage;
        public Atribute2<float> BaseDamage;
        public Atribute2<float> RangedDamage;
        public Atribute2<float> CriticalShot;
        public Atribute2<float> Sharpening;
        public Atribute2<EnergyTypes> EnergyDamage;
        public Atribute<float> AttackSpeed;
        public Atribute2<float> DamageRadius;
        public Atribute2<float> AttackRange;
        public Atribute2<float> Accuracy;
        public Atribute<float> DisruptionMod;
        public Atribute2<float> ShotInLine;
        public Atribute2<float> CriticalMod;
        public Atribute2<float> Strength;
        public Atribute2<float> Holder;
        public Atribute2<SpecialAttackTypes> SpecialAttack;
        public Atribute2<RaretyTypes> Rarity;
        public Atribute2<float> Weight;
        public Atribute2<float> Anomaly;

        public RangedAttackPrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Name = "Ranged Attack",
                Type = SkillGroupTypes.Primary,
                Description = ""
            };
            //------------------------------------------
            DamagePotential = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Damage Potential",
                Type = SkillGroupTypes.Secondary1,
                Description = "Средний урон от оружия с учетом всех модификаторов в реальном бою"
            };
            MovementBreak = new Atribute<float>
            {
                Value = 0,
                Name = "Movement Break",
                Type = SkillGroupTypes.Secondary1,
                Description = "Способность подавить противника огнем и снизить его скорость "
            };
            //------------------------------------------
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
            RangedDamage = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Range Damage",
                Type = SkillGroupTypes.Secondary2,
                Description = "Урон одного выстрела"
            };
            CriticalShot = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Critical Shot",
                Type = SkillGroupTypes.Secondary2,
                Description = "Зависит от критического потенциала оружия и силы навыка Скорости"
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
                Description = "Время в секундах, за которое происходит залп или удар в ближнем бою"
            };
            DamageRadius = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Damage Radius",
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность оружия поражать противников в радиусе попадания выстрела"
            };
            AttackRange = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Attack Range",
                Type = SkillGroupTypes.Secondary2,
                Description = "Дистанция поражения дальнего оружия, соответствует максимуму меткости"
            };
            Accuracy = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Accuracy",
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность попасть в противника, которая снижается с расстоянием"
            };
            DisruptionMod = new Atribute<float>
            {
                Value = 0,
                Name = "Disruption Mod.",
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность дальнего оружия замедлять атакованного противника"
            };
            CriticalMod = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Critical Mod.",
                Type = SkillGroupTypes.Secondary2,
                Description = "Максимальный потенциал увеличения урона при критическом выстреле"
            };
            ShotInLine = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Shot in Line",
                Type = SkillGroupTypes.Secondary2,
                Description = "Количество выстрелов за время скорострельности оружия"
            };
            Strength = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Strength",
                Type = SkillGroupTypes.Secondary2,
                Description = "Некий расчетный потенциал мощности оружия"
            };
            Holder = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Holder",
                Type = SkillGroupTypes.Secondary2,
                Description = "Количество выстрелов до перезарядки длящейся 3 секунды"
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
                Description = "Начиная с редкого, каждая степень редкости дает +30% к базовому урону"
            };
            Weight = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Name = "Weight",
                Type = SkillGroupTypes.Secondary2,
                Description = "Вес оружия повышает чардж и снижает скорость передвижения "
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
            var cp = c.CommonProfile;
            var use2 = false;
            // ReSharper disable once InconsistentNaming
            var l2r = false;
            const WeaponTypes wType = WeaponTypes.Ranged;

            if (wRh != null && wLh != null && wRh.WeaponType == wLh.WeaponType && wRh.WeaponType == wType && wLh.WeaponSize != WeaponSizeTypes.Shield)
            {
                use2 = true;
            }

            if (wRh != null && wLh != null && wRh.WeaponType != wType && wLh.WeaponType == wType && wLh.WeaponSize != WeaponSizeTypes.Shield)
            {
                l2r = true;
            }

            var damage = c.Equipment.GetMeleeDamage(c, wType);
            MeleeDamage.Value1 = l2r ? damage.LeftHandValue: damage.RightHandValue;
            MeleeDamage.Value2 = use2 ? damage.LeftHandValue : 0;

            var criticalDamage = c.Equipment.GetCriticalDamage(c, wType);
            CriticalDamage.Value1 = l2r ? criticalDamage.LeftHandValue: criticalDamage.RightHandValue;
            CriticalDamage.Value2 = use2 ? criticalDamage.LeftHandValue : 0;
            CriticalDamage.UseSecondValue = use2;

            var baseDamage = c.Equipment.GetBaseDamage(wType);
            BaseDamage.Value1 = l2r ? baseDamage.LeftHandValue : baseDamage.RightHandValue;
            BaseDamage.Value2 = use2 ? baseDamage.LeftHandValue : 0;
            BaseDamage.UseSecondValue = use2;

            var oneShotDamage = c.Equipment.GetDamageByWeaponType(c, WeaponTypes.Ranged);
            RangedDamage.Value1 = l2r ? oneShotDamage.LeftHandValue: oneShotDamage.RightHandValue;
            RangedDamage.Value2 = use2 ? oneShotDamage.LeftHandValue : 0;
            RangedDamage.UseSecondValue = use2;

            var criticalShot = c.Equipment.GetCriticalShot(wType);
            CriticalShot.Value1 = l2r ? criticalShot.LeftHandValue : criticalShot.RightHandValue;
            CriticalShot.Value2 = use2 ? criticalShot.LeftHandValue : 0;
            CriticalShot.UseSecondValue = use2;

            var sharpening = c.Equipment.GetSharpening(wType);
            Sharpening.Value1 = l2r ? sharpening.LeftHandValue : sharpening.RightHandValue;
            Sharpening.Value2 = use2 ? sharpening.LeftHandValue : 0;
            Sharpening.UseSecondValue = use2;

            var energyDamage = c.Equipment.GetEnergyDamage(wType);
            EnergyDamage.Value1 = l2r ? energyDamage.LeftHandValue: energyDamage.RightHandValue;
            EnergyDamage.Value2 = use2 ? energyDamage.LeftHandValue : 0;
            EnergyDamage.UseSecondValue = use2;

            var attackSpeed = c.Equipment.GetAttackSpeed(c.CommonProfile, wType);

            var aSpeed1 = attackSpeed.RightHandValue > 0 ? attackSpeed.RightHandValue : 999999;
            var aSpeed2 = attackSpeed.LeftHandValue > 0 ? attackSpeed.LeftHandValue : 999999;
            var aSpeed = aSpeed1 < aSpeed2 ? aSpeed1 : aSpeed2;
            if (aSpeed >= 999999) aSpeed = 0;
            AttackSpeed.Value = aSpeed;
            
            var damageRadius = c.Equipment.GetDamageRadius(c.CommonProfile, wType);
            DamageRadius.Value1 = l2r ? damageRadius.LeftHandValue : damageRadius.RightHandValue;
            DamageRadius.Value2 = use2 ? damageRadius.LeftHandValue : 0;
            DamageRadius.UseSecondValue = use2;

            var attackRange = c.Equipment.GetAttackRange(c.CommonProfile, wType);
            AttackRange.Value1 = l2r ? attackRange.LeftHandValue : attackRange.RightHandValue;
            AttackRange.Value2 = use2 ? attackRange.LeftHandValue : 0;
            AttackRange.UseSecondValue = use2;

            var accuracyMod = 1f;
            if (wRh != null && wLh != null)
            {
                if (wRh.WeaponType == WeaponTypes.Ranged && wLh.WeaponType == WeaponTypes.Ranged)
                {
                    accuracyMod  = 0.75f;
                }
            }
            var accuracy = c.Equipment.GetAccuracy(cp, wType);
            Accuracy.Value1 = l2r ? accuracy.LeftHandValue : accuracy.RightHandValue * accuracyMod;
            Accuracy.Value2 = use2 ? accuracy.LeftHandValue * accuracyMod : 0;
            Accuracy.UseSecondValue = use2;

            var dMod = c.Equipment.GetDisruptionMod(cp, wType).RightHandValue > 0
                ? c.Equipment.GetDisruptionMod(cp, wType).RightHandValue
                : c.Equipment.GetDisruptionMod(cp, wType).LeftHandValue;
            DisruptionMod.Value = dMod;

            var critMod = c.Equipment.GetCriticalMod(cp, wType);
            CriticalMod.Value1 = l2r ? critMod.LeftHandValue: critMod.RightHandValue;
            CriticalMod.Value2 = use2 ? critMod.LeftHandValue : 0;
            CriticalMod.UseSecondValue = use2;

            var shotsInLine = c.Equipment.GetShotInLine(cp, wType);
            ShotInLine.Value1 = l2r ? shotsInLine.LeftHandValue : shotsInLine.RightHandValue;
            ShotInLine.Value2 = use2 ? shotsInLine.LeftHandValue : 0;
            ShotInLine.UseSecondValue = use2;

            var holder = c.Equipment.GetHolder(cp, wType);
            Holder.Value1 = l2r ? holder.LeftHandValue : holder.RightHandValue;
            Holder.Value2 = use2 ? holder.LeftHandValue : 0;
            Holder.UseSecondValue = use2;

            var str = c.Equipment.GetStrength(wType);
            Strength.Value1 = l2r ? str.LeftHandValue: str.RightHandValue;
            Strength.Value2 = use2 ? str.LeftHandValue : 0;
            Strength.UseSecondValue = use2;

            var special = c.Equipment.GetSpecialAttack(wType);
            SpecialAttack.Value1 = l2r ? special.LeftHandValue : special.RightHandValue;
            SpecialAttack.Value2 = use2 ? special.LeftHandValue : 0;
            SpecialAttack.UseSecondValue = use2;

            var rarity = c.Equipment.GetRarityForPlace(wType);
            Rarity.Value1 = l2r ? rarity.LeftHandValue: rarity.RightHandValue;
            Rarity.Value2 = use2 ? rarity.LeftHandValue : 0;
            Rarity.UseSecondValue = use2;

            var weight = c.Equipment.GetWeightFor(wType);
            Weight.Value1 = l2r ? weight.LeftHandValue: weight.RightHandValue;
            Weight.Value2 = use2 ? weight.LeftHandValue : 0;
            Weight.UseSecondValue = use2;

            var anomality = c.Equipment.GetAnomality(c, wType);
            Anomaly.Value1 = l2r ? anomality.LeftHandValue : anomality.RightHandValue;
            Anomaly.Value2 = use2 ? anomality.LeftHandValue : 0;
            Anomaly.UseSecondValue = use2;

            DamagePotential.Value2 = 0;
            DamagePotential.UseSecondValue = false;
            DamagePotential.Value1 = 0;

            //ОДНОРУЧНОЕ В ПРАВОЙ или разнотипные и одно из них милишное
            if ((wRh != null && wLh == null && wRh.WeaponType == WeaponTypes.Ranged) 
                || (wRh != null && wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield && wRh.WeaponType != wLh.WeaponType && (wRh.WeaponType == WeaponTypes.Ranged || wLh.WeaponType == WeaponTypes.Ranged)))
            {
                var weapon = (wRh.WeaponType == WeaponTypes.Ranged) ? wRh : wLh;
                if (weapon != null)
                {
                    DamagePotential.Value2 = 0;
                    DamagePotential.UseSecondValue = false;

                    var a = weapon.GetWeaponDamageByType(c) * (1f / weapon.AttackSpeed);
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
            else if (wRh != null && wRh.WeaponHands == WeaponHandsTypes.TwoHand && wRh.WeaponType == WeaponTypes.Ranged)
            {
                var areaMod = new[] { 1, 1.3f, 1.6f };
                var a = wRh.GetWeaponDamageByType(c) * (1f / wRh.AttackSpeed) * areaMod[wRh.DamageRadius - 1];
                DamagePotential.Value1 = a * (1f + wRh.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.UseSecondValue = false;
                DamagePotential.Value2 = 0;
            }
            // ДВА ОДНОТИПНЫХ - милишных
            else if (wRh != null && wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield && wRh.WeaponType == WeaponTypes.Ranged && wRh.WeaponType == wLh.WeaponType)
            {
                var a1 = wRh.GetWeaponDamageByType(c) * (1f / wRh.AttackSpeed);
                var a2 = wLh.GetWeaponDamageByType(c) * (1f / wLh.AttackSpeed);

                DamagePotential.Value1 = a1 * (1f + wRh.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.Value2 = a2 * (1f + wRh.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
                DamagePotential.UseSecondValue = true;
            }
            // ДВА разнотипных 
            else if (wRh != null && wLh != null && wLh.WeaponSize != WeaponSizeTypes.Shield && (wRh.WeaponType == WeaponTypes.Ranged || wLh.WeaponType == WeaponTypes.Ranged) && wRh.WeaponType != wLh.WeaponType)
            {
                var weapon = (wRh.WeaponType == WeaponTypes.Melee) ? wRh : wLh;
                DamagePotential.Value2 = 0;
                DamagePotential.UseSecondValue = false;

                var a = weapon.GetWeaponDamageByType(c) * (1f / weapon.AttackSpeed);
                DamagePotential.Value1 = a * (1f + weapon.CriticalModifier * (c.CommonProfile.Characteristics.Speed.Percent / 100f) * (c.CommonProfile.Characteristics.Speed.Strenght / 100f));
            }

            // Помехи
            var abilityBreak1 = 99999999999f;
            var abilityBreak2 = 99999999999f;
            if (wRh != null && wRh.WeaponType == WeaponTypes.Ranged)
            {
                abilityBreak1 = CommonCharacterProfile.CommonAbv[c.Level] * wRh.DisruptionModifier;
            }
            if (wLh != null && wLh.WeaponType == WeaponTypes.Ranged)
            {
                abilityBreak2 = CommonCharacterProfile.CommonAbv[c.Level] * wLh.DisruptionModifier;
            }
            var abiBreak = Math.Min(abilityBreak1, abilityBreak2);
            if (abiBreak > 9999999999f) abiBreak = 0;
            MovementBreak.Value = abiBreak;
        }
    }
}
