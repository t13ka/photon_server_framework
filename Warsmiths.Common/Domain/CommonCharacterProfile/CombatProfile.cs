using Warsmiths.Common.Domain.CommonCharacterProfile.Attributes;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Utils;

namespace Warsmiths.Common.Domain.CommonCharacterProfile
{
    public class CombatMelee
    {
        public Atribute<int> Attack;
        public Atribute<int> AbilityBreak;
        public Atribute2<int> Damage;
        public Atribute<int> CriticalDamage;
        public Atribute2<EnergyTypes> EnergyDamage;
        public Atribute<float> AttackSpeed; //
        public Atribute2<int> AttackArea;
        public Atribute<int> AttackDistance; //
        public Atribute<int> MaxTargets;
        public Atribute2<SpecialAttackTypes> SpecialAttack;

        public CombatMelee()
        {
            Attack = new Atribute<int>();
            AbilityBreak = new Atribute<int>();
            Damage = new Atribute2<int>();
            CriticalDamage = new Atribute<int>();
            EnergyDamage = new Atribute2<EnergyTypes>();
            AttackSpeed = new Atribute<float>();
            AttackArea = new Atribute2<int>();
            AttackDistance = new Atribute<int>();
            MaxTargets = new Atribute<int>();
            SpecialAttack = new Atribute2<SpecialAttackTypes>();
        }
    }

    public class CombatRangedMagic
    {
        public Atribute<int> Attack;
        public Atribute<int> MovementBreak;
        public Atribute2<int> Damage;
        public Atribute<int> CriticalDamage;
        public Atribute2<EnergyTypes> EnergyDamage;
        public Atribute<float> AttackSpeed; // 
        public Atribute2<float> AttackRange; //
        public Atribute2<int> DamageRadius;
        public Atribute2<float> Accuracy; //
        public Atribute2<int> ShotsInLine;
        public Atribute2<int> Holder;
        public Atribute2<int> Strength;
        public Atribute2<SpecialAttackTypes> SpecialAttack;

        public Atribute<int> SpiritBreak;
        public Atribute<int> MaxTargets;

        public CombatRangedMagic()
        {
            Attack = new Atribute<int>();
            MovementBreak = new Atribute<int>();
            Damage = new Atribute2<int>();
            CriticalDamage = new Atribute<int>();
            EnergyDamage = new Atribute2<EnergyTypes>();
            AttackSpeed = new Atribute<float>();
            DamageRadius = new Atribute2<int>();
            Accuracy = new Atribute2<float>();
            ShotsInLine = new Atribute2<int>();
            Holder = new Atribute2<int>();
            Strength = new Atribute2<int>();
            SpecialAttack = new Atribute2<SpecialAttackTypes>();
            SpiritBreak = new Atribute<int>();
            MaxTargets = new Atribute<int>();
            AttackRange = new Atribute2<float>();
        }
    }

    public class CombatCommon
    {
        public Atribute<int> MeleeDefence;
        public Atribute<int> TotalDurability;
        public Atribute<int> AbilityResist;

        public Atribute<int> CritChance;

        public Atribute4<int> Chest;
        public Atribute4<int> Back;
        public Atribute4<int> RightHand;
        public Atribute4<int> LeftHand;
        public Atribute4<int> LeftLeg;
        public Atribute4<int> RightLeg;

        public Atribute<int> FireShield;
        public Atribute<int> IceShield;
        public Atribute<int> ElectricityShield;
        public Atribute<int> MeleeArmor;

        public Atribute<int> AttackAngle;
        public Atribute<ArmorTypes> Type;
        public Atribute<StructureTypes> Structure;
        public Atribute<int> ActiveTriggers;
        public Atribute<int> ActiveModules;

        public Atribute<string> SpecialDefense;

        public Atribute<int> RangedDefence;
        public Atribute<int> MovementResist;
        public Atribute<int> RangedArmor;

        public Atribute<int> MagicDefence;
        public Atribute<int> SpiritResist;
        public Atribute<int> MagicArmor;

        public Atribute<int> Mind;
        public Atribute<int> Initiative;
        public Atribute<int> Immunity;
        public Atribute<int> Stamina;

        public Atribute<int> Body;
        public Atribute<float> Movement;
        public Atribute<int> Charge;
        public Atribute<int> Vision;

        public Atribute<int> Autonomy;
        public Atribute<int> TotalAnomaly;
        public Atribute<int> TotalWeight;

        public Atribute<int> Reversal;
        public Atribute<int> ReversalChance;
        /*public Atribute<float> MeleeCritStrength;
        public Atribute<float> RangedCritStrength;*/

        public float CombatPotential;

        public CombatCommon()
        {
            MeleeDefence = new Atribute<int>();
            TotalDurability = new Atribute<int>();
            AbilityResist = new Atribute<int>();
            CritChance = new Atribute<int>();

            Chest = new Atribute4<int>();
            Back = new Atribute4<int>();
            RightHand = new Atribute4<int>();
            LeftHand = new Atribute4<int>();
            LeftLeg = new Atribute4<int>();
            RightLeg = new Atribute4<int>();

            FireShield = new Atribute<int>();
            IceShield = new Atribute<int>();
            ElectricityShield = new Atribute<int>();
            MeleeArmor = new Atribute<int>();

            AttackAngle = new Atribute<int>();
            Type = new Atribute<ArmorTypes>();
            Structure = new Atribute<StructureTypes>();
            ActiveTriggers = new Atribute<int>();
            ActiveModules = new Atribute<int>();

            SpecialDefense = new Atribute<string>();

            RangedDefence = new Atribute<int>();
            MovementResist = new Atribute<int>();
            RangedArmor = new Atribute<int>();

            MagicDefence = new Atribute<int>();
            SpiritResist = new Atribute<int>();
            MagicArmor = new Atribute<int>();

            Mind = new Atribute<int>();
            Initiative = new Atribute<int>();
            Immunity = new Atribute<int>();
            Stamina = new Atribute<int>();

            Body = new Atribute<int>();
            Movement = new Atribute<float>();
            Charge = new Atribute<int>();
            Vision = new Atribute<int>();

            Autonomy = new Atribute<int>();
            TotalAnomaly = new Atribute<int>();
            TotalWeight = new Atribute<int>();

            Reversal = new Atribute<int>();
            ReversalChance = new Atribute<int>();

            /*MeleeCritStrength = new Atribute<float>();
            RangedCritStrength = new Atribute<float>();*/

            CombatPotential = 0;
        }
    }


    public enum WeaponCombinationsE
    {
        None = 0,

        Sword,
        Riffle,
        Stuff,

        SwordTwoHand,
        RiffleTwoHand,
        StuffTwoHand,

        SwordShield,
        RiffleShield,
        StuffShield,

        SwordSword,
        RiffleRiffle,
        StuffStuff, 

        SwordRiffle,
        SwordStuff,
    }

    

    public class CombatProfile
    {
        public const float ScaleModifier = 2f;

        public WeaponCombinationsE WeaponCombination;

        public CombatMelee Melee;
        public CombatRangedMagic RangedMagic;
        public CombatCommon Common;

        public bool IsBaseCombination()
        {
            return (WeaponCombination != WeaponCombinationsE.None) &&
                   (WeaponCombination == WeaponCombinationsE.Sword ||
                    WeaponCombination == WeaponCombinationsE.Riffle ||
                    WeaponCombination == WeaponCombinationsE.Stuff ||
                    WeaponCombination == WeaponCombinationsE.SwordTwoHand ||
                    WeaponCombination == WeaponCombinationsE.RiffleTwoHand ||
                    WeaponCombination == WeaponCombinationsE.StuffTwoHand ||
                    WeaponCombination == WeaponCombinationsE.RiffleRiffle ||
                    WeaponCombination == WeaponCombinationsE.SwordRiffle ||
                    WeaponCombination == WeaponCombinationsE.SwordStuff);

        }

        public bool IsShieldCombination()
        {
            return (WeaponCombination != WeaponCombinationsE.None) &&
                   (WeaponCombination == WeaponCombinationsE.SwordShield ||
                    WeaponCombination == WeaponCombinationsE.RiffleShield ||
                    WeaponCombination == WeaponCombinationsE.StuffShield);

        }

        public bool IsTwoSwordsCombination()
        {
            return (WeaponCombination != WeaponCombinationsE.None) &&
                   (WeaponCombination == WeaponCombinationsE.SwordSword);

        }

        public bool IsTwoStuffCombination()
        {
            return (WeaponCombination != WeaponCombinationsE.None) &&
                   (WeaponCombination == WeaponCombinationsE.StuffStuff);

        }

        public CombatProfile()
        {
            Melee = new CombatMelee();
            RangedMagic = new CombatRangedMagic();
            Common = new CombatCommon();
        }

        public void Reset(Character c)
        {
            // Check combinations
            var w1 = c.Equipment.RightWeapon;
            var w2 = c.Equipment.LeftWeapon;
            WeaponCombination = WeaponCombinationsE.None;
            if (w1 != null)
            {
                if (w2 == null)
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (w1.WeaponType)
                    {
                        case WeaponTypes.Melee:
                            WeaponCombination = w1.WeaponHands == WeaponHandsTypes.OneHand
                                ? WeaponCombinationsE.Sword
                                : WeaponCombinationsE.SwordTwoHand;
                            break;
                        case WeaponTypes.Ranged:
                            WeaponCombination = w1.WeaponHands == WeaponHandsTypes.OneHand
                                ? WeaponCombinationsE.Riffle
                                : WeaponCombinationsE.RiffleTwoHand;
                            break;
                        case WeaponTypes.Magic:
                            WeaponCombination = w1.WeaponHands == WeaponHandsTypes.OneHand
                                ? WeaponCombinationsE.Stuff
                                : WeaponCombinationsE.StuffTwoHand;
                            break;
                    }
                }
                else
                {
                    if (w2.WeaponSize == WeaponSizeTypes.Shield)
                    {
                        switch (w1.WeaponType)
                        {
                            case WeaponTypes.Melee:
                                WeaponCombination = WeaponCombinationsE.SwordShield;
                                break;
                            case WeaponTypes.Ranged:
                                WeaponCombination = WeaponCombinationsE.RiffleShield;
                                break;
                            case WeaponTypes.Magic:
                                WeaponCombination = WeaponCombinationsE.StuffShield;
                                break;
                        }
                    }
                    else
                    {
                        if (w1.WeaponType == WeaponTypes.Melee && w2.WeaponType == WeaponTypes.Melee) WeaponCombination = WeaponCombinationsE.SwordSword;
                        if (w1.WeaponType == WeaponTypes.Melee && w2.WeaponType == WeaponTypes.Ranged) WeaponCombination = WeaponCombinationsE.SwordRiffle;
                        if (w1.WeaponType == WeaponTypes.Melee && w2.WeaponType == WeaponTypes.Magic) WeaponCombination = WeaponCombinationsE.SwordStuff;
                        if (w1.WeaponType == WeaponTypes.Ranged && w2.WeaponType == WeaponTypes.Melee) WeaponCombination = WeaponCombinationsE.SwordRiffle;
                        if (w1.WeaponType == WeaponTypes.Ranged && w2.WeaponType == WeaponTypes.Ranged) WeaponCombination = WeaponCombinationsE.RiffleRiffle;
                        if (w1.WeaponType == WeaponTypes.Magic && w2.WeaponType == WeaponTypes.Melee) WeaponCombination = WeaponCombinationsE.SwordStuff;
                        if (w1.WeaponType == WeaponTypes.Magic && w2.WeaponType == WeaponTypes.Magic) WeaponCombination = WeaponCombinationsE.StuffStuff;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            Melee.Attack.Value = c.CommonProfile.Offence.MeleeAttack.Value.Value.RoundToInt();
            Melee.AbilityBreak.Value = c.CommonProfile.Offence.MeleeAttack.AbilityBreak.Value.RoundToInt();

            Melee.Damage.Value1 = c.CommonProfile.Offence.MeleeAttack.MeleeDamage.Value1.RoundToInt();
            Melee.Damage.UseSecondValue = c.CommonProfile.Offence.MeleeAttack.MeleeDamage.UseSecondValue;
            Melee.Damage.Value2 = c.CommonProfile.Offence.MeleeAttack.MeleeDamage.Value2.RoundToInt();

            Melee.CriticalDamage.Value = c.CommonProfile.Hidden.RangedCritStrength.RoundToInt();
            /*c.CommonProfile.Offence.MeleeAttack.CriticalDamage.Value1;
            Melee.CriticalDamage.UseSecondValue = c.CommonProfile.Offence.MeleeAttack.CriticalDamage.UseSecondValue;
            Melee.CriticalDamage.Value2 = c.CommonProfile.Offence.MeleeAttack.CriticalDamage.Value2;*/

            

            Melee.EnergyDamage.Value1 = c.CommonProfile.Offence.MeleeAttack.EnergyDamage.Value1;
            Melee.EnergyDamage.UseSecondValue = c.CommonProfile.Offence.MeleeAttack.EnergyDamage.UseSecondValue;
            Melee.EnergyDamage.Value2 = c.CommonProfile.Offence.MeleeAttack.EnergyDamage.Value2;

            Melee.AttackSpeed.Value = c.CommonProfile.Offence.MeleeAttack.AttackSpeed.Value;

            Melee.AttackArea.Value1 = c.CommonProfile.Offence.MeleeAttack.AttackArea.Value1;
            Melee.AttackArea.UseSecondValue = c.CommonProfile.Offence.MeleeAttack.AttackArea.UseSecondValue;
            Melee.AttackArea.Value2 = c.CommonProfile.Offence.MeleeAttack.AttackArea.Value2;

            Melee.AttackDistance.Value = c.CommonProfile.Offence.MeleeAttack.AttackDistance.Value.RoundToInt();

            Melee.MaxTargets.Value = c.CommonProfile.Offence.MeleeAttack.MaxTargets.Value;

            Melee.SpecialAttack.Value1 = c.CommonProfile.Offence.MeleeAttack.SpecialAttack.Value1;
            Melee.SpecialAttack.UseSecondValue = c.CommonProfile.Offence.MeleeAttack.SpecialAttack.UseSecondValue;
            Melee.SpecialAttack.Value2 = c.CommonProfile.Offence.MeleeAttack.SpecialAttack.Value1;

            ////////////////////////////////////////////////////////////////////////////////////////////////////

            var hasRiffle = (c.Equipment?.RightWeapon != null && c.Equipment.RightWeapon.WeaponType == WeaponTypes.Ranged) ||
                            (c.Equipment?.LeftWeapon != null && c.Equipment.LeftWeapon.WeaponType == WeaponTypes.Ranged);
            var hasStaff = (c.Equipment?.RightWeapon != null && c.Equipment.RightWeapon.WeaponType == WeaponTypes.Magic) ||
                            (c.Equipment?.LeftWeapon != null && c.Equipment.LeftWeapon.WeaponType == WeaponTypes.Magic);

            if (hasRiffle)
            {
                RangedMagic.Attack.Value = c.CommonProfile.Offence.RangedAttack.Value.Value.RoundToInt();
                RangedMagic.MovementBreak.Value = c.CommonProfile.Offence.RangedAttack.MovementBreak.Value.RoundToInt();

                RangedMagic.Damage.Value1 = c.CommonProfile.Offence.RangedAttack.RangedDamage.Value1.RoundToInt();
                RangedMagic.Damage.UseSecondValue = c.CommonProfile.Offence.RangedAttack.RangedDamage.UseSecondValue;
                RangedMagic.Damage.Value2 = c.CommonProfile.Offence.RangedAttack.RangedDamage.Value2.RoundToInt();

                RangedMagic.AttackRange.Value1 = c.CommonProfile.Offence.RangedAttack.AttackRange.Value1 / ScaleModifier;
                RangedMagic.AttackRange.UseSecondValue = c.CommonProfile.Offence.RangedAttack.AttackRange.UseSecondValue;
                RangedMagic.AttackRange.Value2 = c.CommonProfile.Offence.RangedAttack.AttackRange.Value2 / ScaleModifier;

                RangedMagic.CriticalDamage.Value = c.CommonProfile.Hidden.RangedCritStrength.RoundToInt();
                /*c.CommonProfile.Offence.RangedAttack.CriticalDamage.Value1;
                RangedMagic.CriticalDamage.UseSecondValue =
                    c.CommonProfile.Offence.RangedAttack.CriticalDamage.UseSecondValue;
                RangedMagic.CriticalDamage.Value2 = c.CommonProfile.Offence.RangedAttack.CriticalDamage.Value2;
                */
                RangedMagic.EnergyDamage.Value1 = c.CommonProfile.Offence.RangedAttack.EnergyDamage.Value1;
                RangedMagic.EnergyDamage.UseSecondValue =
                    c.CommonProfile.Offence.RangedAttack.EnergyDamage.UseSecondValue;
                RangedMagic.EnergyDamage.Value2 = c.CommonProfile.Offence.RangedAttack.EnergyDamage.Value2;

                RangedMagic.AttackSpeed.Value = c.CommonProfile.Offence.RangedAttack.AttackSpeed.Value;

                RangedMagic.DamageRadius.Value1 = c.CommonProfile.Offence.RangedAttack.DamageRadius.Value1.RoundToInt();
                RangedMagic.DamageRadius.UseSecondValue =
                    c.CommonProfile.Offence.RangedAttack.DamageRadius.UseSecondValue;
                RangedMagic.DamageRadius.Value2 = c.CommonProfile.Offence.RangedAttack.DamageRadius.Value2.RoundToInt();

                RangedMagic.Accuracy.Value1 = c.CommonProfile.Offence.RangedAttack.Accuracy.Value1;
                RangedMagic.Accuracy.UseSecondValue = c.CommonProfile.Offence.RangedAttack.Accuracy.UseSecondValue;
                RangedMagic.Accuracy.Value2 = c.CommonProfile.Offence.RangedAttack.Accuracy.Value2;

                RangedMagic.ShotsInLine.Value1 = c.CommonProfile.Offence.RangedAttack.ShotInLine.Value1.RoundToInt();
                RangedMagic.ShotsInLine.UseSecondValue = c.CommonProfile.Offence.RangedAttack.ShotInLine.UseSecondValue;
                RangedMagic.ShotsInLine.Value2 = c.CommonProfile.Offence.RangedAttack.ShotInLine.Value2.RoundToInt();

                RangedMagic.Holder.Value1 = c.CommonProfile.Offence.RangedAttack.Holder.Value1.RoundToInt();
                RangedMagic.Holder.UseSecondValue = c.CommonProfile.Offence.RangedAttack.Holder.UseSecondValue;
                RangedMagic.Holder.Value2 = c.CommonProfile.Offence.RangedAttack.Holder.Value2.RoundToInt();

                RangedMagic.Strength.Value1 = c.CommonProfile.Offence.RangedAttack.Strength.Value1.RoundToInt();
                RangedMagic.Strength.UseSecondValue = c.CommonProfile.Offence.RangedAttack.Strength.UseSecondValue;
                RangedMagic.Strength.Value2 = c.CommonProfile.Offence.RangedAttack.Strength.Value2.RoundToInt();

                RangedMagic.SpecialAttack.Value1 = c.CommonProfile.Offence.RangedAttack.SpecialAttack.Value1;
                RangedMagic.SpecialAttack.UseSecondValue =
                    c.CommonProfile.Offence.RangedAttack.SpecialAttack.UseSecondValue;
                RangedMagic.SpecialAttack.Value2 = c.CommonProfile.Offence.RangedAttack.SpecialAttack.Value2;
            }
            if (hasStaff)
            {
                RangedMagic.Attack.Value = c.CommonProfile.Offence.MagicAttack.Value.Value.RoundToInt();

                RangedMagic.Damage.Value1 = c.CommonProfile.Offence.MagicAttack.MagicDamage.Value.RoundToInt();
                RangedMagic.Damage.UseSecondValue = false;
                RangedMagic.Damage.Value2 = 0;

                RangedMagic.CriticalDamage.Value = c.CommonProfile.Hidden.RangedCritStrength.RoundToInt();

                /*c.CommonProfile.Offence.MagicAttack.CriticalDamage.Value1;
                RangedMagic.CriticalDamage.UseSecondValue = c.CommonProfile.Offence.MagicAttack.CriticalDamage.UseSecondValue;
                RangedMagic.CriticalDamage.Value2 = c.CommonProfile.Offence.MagicAttack.CriticalDamage.Value2;*/

                RangedMagic.AttackRange.Value1 = 0;
                RangedMagic.AttackRange.UseSecondValue = false;
                RangedMagic.AttackRange.Value2 = 0;

                RangedMagic.EnergyDamage.Value1 = c.CommonProfile.Offence.MagicAttack.EnergyDamage.Value1;
                RangedMagic.EnergyDamage.UseSecondValue = c.CommonProfile.Offence.MagicAttack.EnergyDamage.UseSecondValue;
                RangedMagic.EnergyDamage.Value2 = c.CommonProfile.Offence.MagicAttack.EnergyDamage.Value2;

                RangedMagic.AttackSpeed.Value = c.CommonProfile.Offence.MagicAttack.AttackSpeed.Value;

                RangedMagic.DamageRadius.Value1 = 0;
                RangedMagic.DamageRadius.UseSecondValue = false;
                RangedMagic.DamageRadius.Value2 = 0;

                RangedMagic.Accuracy.Value1 = 0;
                RangedMagic.Accuracy.UseSecondValue = false;
                RangedMagic.Accuracy.Value2 = 0;

                RangedMagic.ShotsInLine.Value1 = 0;
                RangedMagic.ShotsInLine.UseSecondValue = false;
                RangedMagic.ShotsInLine.Value2 = 0;

                RangedMagic.Holder.Value1 = c.CommonProfile.Offence.MagicAttack.Charger.Value.RoundToInt();
                RangedMagic.Holder.UseSecondValue = false;
                RangedMagic.Holder.Value2 = 0;

                RangedMagic.Strength.Value1 = 0;
                RangedMagic.Strength.UseSecondValue = false;
                RangedMagic.Strength.Value2 = 0;

                RangedMagic.SpecialAttack.Value1 = c.CommonProfile.Offence.MagicAttack.SpecialAttack.Value1;
                RangedMagic.SpecialAttack.UseSecondValue =
                    c.CommonProfile.Offence.MagicAttack.SpecialAttack.UseSecondValue;
                RangedMagic.SpecialAttack.Value2 = c.CommonProfile.Offence.MagicAttack.SpecialAttack.Value2;

                RangedMagic.SpiritBreak.Value = c.CommonProfile.Offence.MagicAttack.SpiritBreak.Value.RoundToInt();
                RangedMagic.MaxTargets.Value = c.CommonProfile.Offence.MagicAttack.MaxTargets.Value.RoundToInt();
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            Common.MeleeDefence.Value = c.CommonProfile.Defence.MeleeDefense.Value.Value.RoundToInt();
            Common.TotalDurability.Value = c.CommonProfile.Defence.MeleeDefense.TotalDurability.Value.RoundToInt();
            Common.AbilityResist.Value = c.CommonProfile.Defence.MeleeDefense.AbilityResist.Value.RoundToInt();
            Common.CritChance.Value = c.CommonProfile.Characteristics.Speed.Percent;
            Common.Chest.Value1 = c.CommonProfile.Defence.MeleeDefense.Chest.Value1.RoundToInt();
            Common.Chest.Value2 = c.CommonProfile.Defence.MeleeDefense.Chest.Value2.RoundToInt();
            Common.Chest.Value3 = c.CommonProfile.Defence.MeleeDefense.Chest.Value3.RoundToInt();
            Common.Chest.Value4 = c.CommonProfile.Defence.MeleeDefense.Chest.Value4.RoundToInt();
            Common.Back.Value1 = c.CommonProfile.Defence.MeleeDefense.Back.Value1.RoundToInt();
            Common.Back.Value2 = c.CommonProfile.Defence.MeleeDefense.Back.Value2.RoundToInt();
            Common.Back.Value3 = c.CommonProfile.Defence.MeleeDefense.Back.Value3.RoundToInt();
            Common.Back.Value4 = c.CommonProfile.Defence.MeleeDefense.Back.Value4.RoundToInt();
            Common.RightHand.Value1 = c.CommonProfile.Defence.MeleeDefense.RightHand.Value1.RoundToInt();
            Common.RightHand.Value2 = c.CommonProfile.Defence.MeleeDefense.RightHand.Value2.RoundToInt();
            Common.RightHand.Value3 = c.CommonProfile.Defence.MeleeDefense.RightHand.Value3.RoundToInt();
            Common.RightHand.Value4 = c.CommonProfile.Defence.MeleeDefense.RightHand.Value4.RoundToInt();
            Common.LeftHand.Value1 = c.CommonProfile.Defence.MeleeDefense.LeftHand.Value1.RoundToInt();
            Common.LeftHand.Value2 = c.CommonProfile.Defence.MeleeDefense.LeftHand.Value2.RoundToInt();
            Common.LeftHand.Value3 = c.CommonProfile.Defence.MeleeDefense.LeftHand.Value3.RoundToInt();
            Common.LeftHand.Value4 = c.CommonProfile.Defence.MeleeDefense.LeftHand.Value4.RoundToInt();
            Common.LeftLeg.Value1 = c.CommonProfile.Defence.MeleeDefense.LeftLeg.Value1.RoundToInt();
            Common.LeftLeg.Value2 = c.CommonProfile.Defence.MeleeDefense.LeftLeg.Value2.RoundToInt();
            Common.LeftLeg.Value3 = c.CommonProfile.Defence.MeleeDefense.LeftLeg.Value3.RoundToInt();
            Common.LeftLeg.Value4 = c.CommonProfile.Defence.MeleeDefense.LeftLeg.Value4.RoundToInt();
            Common.RightLeg.Value1 = c.CommonProfile.Defence.MeleeDefense.RightLeg.Value1.RoundToInt();
            Common.RightLeg.Value2 = c.CommonProfile.Defence.MeleeDefense.RightLeg.Value2.RoundToInt();
            Common.RightLeg.Value3 = c.CommonProfile.Defence.MeleeDefense.RightLeg.Value3.RoundToInt();
            Common.RightLeg.Value4 = c.CommonProfile.Defence.MeleeDefense.RightLeg.Value4.RoundToInt();

            Common.FireShield.Value = c.CommonProfile.Defence.MeleeDefense.FireShield.Value.RoundToInt();
            Common.IceShield.Value = c.CommonProfile.Defence.MeleeDefense.IceShield.Value.RoundToInt();
            Common.ElectricityShield.Value = c.CommonProfile.Defence.MeleeDefense.ElectricityShield.Value.RoundToInt();
            Common.MeleeArmor.Value = c.CommonProfile.Defence.MeleeDefense.Armor.Value.RoundToInt();

            Common.AttackAngle.Value = c.CommonProfile.Defence.MeleeDefense.AttackAngle.Value.RoundToInt();
            Common.Type.Value = c.CommonProfile.Defence.MeleeDefense.Type.Value;
            Common.Structure.Value = c.CommonProfile.Defence.MeleeDefense.Structure.Value;
            Common.ActiveTriggers.Value = c.CommonProfile.Defence.MeleeDefense.ActiveTriggers.Value.RoundToInt();
            Common.ActiveModules.Value = c.CommonProfile.Defence.MeleeDefense.ActiveModules.Value.RoundToInt();

            Common.RangedDefence.Value = c.CommonProfile.Defence.RangedDefense.Value.Value.RoundToInt();
            Common.MovementResist.Value = c.CommonProfile.Defence.RangedDefense.MovementResist.Value.RoundToInt();
            Common.RangedArmor.Value = c.CommonProfile.Defence.RangedDefense.Armor.Value.RoundToInt();

            Common.MagicDefence.Value = c.CommonProfile.Defence.MagicDefense.Value.Value.RoundToInt();
            Common.SpiritResist.Value = c.CommonProfile.Defence.MagicDefense.SpiritResist.Value.RoundToInt();
            Common.MagicArmor.Value = c.CommonProfile.Defence.MagicDefense.Armor.Value.RoundToInt();

            // TODO: Refactror when specdef will come to us...
            Common.SpecialDefense.Value = c.CommonProfile.Defence.MeleeDefense.SpecialDefense.Value + c.CommonProfile.Defence.RangedDefense.SpecialDefense.Value + c.CommonProfile.Defence.MagicDefense.SpecialDefense.Value;

            Common.Mind.Value = c.CommonProfile.Mastery.Mind.Value.Value.RoundToInt();

            var imunCalc = c.CommonProfile.Mastery.Mind.Initiative.Value;
            imunCalc = (4550 - 19 * imunCalc) / 180f;
            if (imunCalc < 1) imunCalc = 1;
            Common.Initiative.Value = imunCalc.RoundToInt();

            Common.Immunity.Value = c.CommonProfile.Mastery.Mind.Immunity.Value.RoundToInt();
            Common.Stamina.Value = (c.CommonProfile.Mastery.Mind.Stamina.Value / 25f).RoundToInt();
            
            Common.Body.Value = c.CommonProfile.Mastery.Body.Value.Value.RoundToInt();
            Common.Movement.Value = c.CommonProfile.Mastery.Body.Movement.Value / 30f;
            Common.Charge.Value = (c.CommonProfile.Mastery.Body.Charge.Value / 3f).RoundToInt();
            Common.Vision.Value = (c.CommonProfile.Mastery.Body.Vision.Value / 20f).RoundToInt();

            Common.Autonomy.Value = c.CommonProfile.Mastery.Skill.Autonomy.Value.RoundToInt();
            Common.TotalAnomaly.Value = c.CommonProfile.Mastery.Skill.TotalAnomaly.Value.RoundToInt();
            Common.TotalWeight.Value = c.CommonProfile.Mastery.Skill.TotalWeight.Value.RoundToInt();

            Common.Reversal.Value = c.CommonProfile.Hidden.Reversal.RoundToInt();
            Common.ReversalChance.Value = c.CommonProfile.Hidden.ReversalChance.RoundToInt();
            /*Common.MeleeCritStrength.Value = c.CommonProfile.Hidden.MeleeCritStrength;
            Common.RangedCritStrength.Value = c.CommonProfile.Hidden.RangedCritStrength;*/

            Common.CombatPotential = c.CommonProfile.CombatPotential;
        }
    }
}