using System;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Results;
using Warsmiths.Common.Utils;
// ReSharper disable MergeConditionalExpression
// ReSharper disable UseStringInterpolation

namespace Warsmiths.Common.Domain.Equipment
{
    public class BaseWeapon : BaseEquipment
    {
        #region Fields

        #endregion

        #region Ctor

        public BaseWeapon(string name)
        {
            _id = Guid.NewGuid().ToString(); 
            Name = name;
        }

        public override BaseEquipment Clone()
        {
            return new BaseWeapon
            {
                _id = Guid.NewGuid().ToString(),
                RaceType = RaceType,
                Rarety = Rarety,
                EnergyTypes = EnergyTypes,
                ColorType = ColorType,
                Prok = Prok,
                Anomality = Anomality,
                Weight = Weight,
                Sharpening = Sharpening,
                Price = Price,
                Sprite = Sprite,
                Charger = Charger,
                CriticalBlast = CriticalBlast,
                EnergyDamageFire = EnergyDamageFire,
                EnergyDamageIce = EnergyDamageIce,
                EnergyDamageElectro = EnergyDamageElectro,
                Chance = Chance,
                Accuracy = Accuracy,
                AttackSpeed = AttackSpeed,
                Holder = Holder,
                AttackRange = AttackRange,
                MaxTargets = MaxTargets,
                DisruptionModifier = DisruptionModifier,
                WeaponType = WeaponType,
                AttackArea = AttackArea,
                SpecialAttack = SpecialAttack,
                DamageRadius = DamageRadius,
                BaseDamage = BaseDamage,
                OneShotDamage = OneShotDamage,
                ShotsInLine = ShotsInLine,
                CriticalShot = CriticalShot,
                CriticalModifier = CriticalModifier,
                Strength = Strength,
                WeaponHands = WeaponHands,
                WeaponSize = WeaponSize,
                Name = Name,
            };
        }

        public BaseWeapon()
        {
            _id = Guid.NewGuid().ToString(); 
            Sprite = "";
        }

        #endregion

        #region Props
        public float Charger ;
        public float CriticalBlast ;
        public float EnergyDamageFire ;
        public float EnergyDamageIce ;
        public float EnergyDamageElectro ;
        public float Accuracy ;
        public float AttackSpeed ;
        public float Holder ;
        public float AttackRange ;
        public int MaxTargets ;
        public float DisruptionModifier ;
        public WeaponTypes WeaponType ;
        public int AttackArea ;
        public SpecialAttackTypes SpecialAttack ;
        public int DamageRadius ;
        public float BaseDamage ;
        public float OneShotDamage ;
        public float ShotsInLine ;
        public float CriticalShot ;
        public float CriticalModifier ;
        public float Strength ;
        public WeaponHandsTypes WeaponHands ;
        public WeaponSizeTypes WeaponSize ;

        #endregion

        #region Methods

        public float GetTotalPrice()
        {
            float rareMod1;
            float rareMod2;
            switch (Rarety)
            {
                case RaretyTypes.Regular:
                    rareMod1 = 1f;
                    rareMod2 = 1;
                    break;
                case RaretyTypes.Rare:
                    rareMod1 = 5f;
                    rareMod2 = 1.1f;
                    break;
                case RaretyTypes.Epic:
                    rareMod1 = 10f;
                    rareMod2 = 1.3f;
                    break;
                case RaretyTypes.Legend:
                    rareMod1 = 20f;
                    rareMod2 = 1.5f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var resultPrice = (Price*rareMod1) + GetSharpeningTotalPrice()*rareMod2;
            resultPrice = resultPrice - (resultPrice*Prok)/100;
            return resultPrice;
        }

        public float GetMaxOpponentsModifier()
        {
            var maxOpponentsModifier = 0f;
            if (MaxTargets == 2)
            {
                maxOpponentsModifier = 1.3f;
            }
            if (MaxTargets == 3)
            {
                maxOpponentsModifier = 1.5f;
            }

            return maxOpponentsModifier;
        }

        public float GetAttackAreaModifier()
        {
            var attackAreaModifier = (float)AttackArea;
            if (AttackArea == 2)
            {
                attackAreaModifier = 1.3f;
            }
            if (AttackArea == 3)
            {
                attackAreaModifier = 1.5f;
            }

            return attackAreaModifier;
        }

        public RaretyTypes MaxRarityForLevel(int level)
        {
            if (level <= 3) return RaretyTypes.Regular;
            if (level <= 6) return RaretyTypes.Rare;
            return level <= 9 ? RaretyTypes.Epic : RaretyTypes.Legend;
        }

        public float GetRaretyModifier(int charLevel, bool useLevelMod)
        {
            var modRarety = 1f;
            var allowRarity = MaxRarityForLevel(charLevel);
            var r = useLevelMod ? (Rarety > allowRarity ? allowRarity : Rarety) : Rarety;
            switch (r)
            {
                case RaretyTypes.Rare:
                    modRarety = 1.3f;
                    break;
                case RaretyTypes.Epic:
                    modRarety = 1.6f;
                    break;
                case RaretyTypes.Legend:
                    modRarety = 1.9f;
                    break;
                case RaretyTypes.Regular:
                    modRarety = 1f;
                    break;
            }

            return modRarety;
        }

        public float GetMeleeDamage(Character character)
        {
            var levelMod = DomainConfiguration.EquipmentLevelPercents[character.Level];
            // УРОН= (базовый урон/7.8*значение уровня)*Мод.редкости(1,1.3,1.6,1.9)*(1+ %заточки/100)
            var result = (BaseDamage / 7.8f * levelMod) * GetRaretyModifier(character.Level, true) * (1f + Sharpening / 100f);
            return result;
        }

        public float GetMagicDamage(Character character)
        {
            var levelMod = DomainConfiguration.EquipmentLevelPercents[character.Level];

            // УРОН ЗАРЯДА - (базовый урон/7.8*значение уровня)*Мод.редкости(1,1.3,1.6,1.9)*(1+ %заточки/100)/3
            var result = (BaseDamage / 7.8f * levelMod) * GetRaretyModifier(character.Level, true) * (1 + Sharpening / 100f) / 3;
            return result;
        }

        public float GetRangeDamage(Character character)
        {
            var levelMod = DomainConfiguration.EquipmentLevelPercents[character.Level];

            // УРОН ВЫСТРЕЛА – (базовый урон/7.8*значение уровня)*Мод.редкости(1,1.3,1.6,1.9)*(1+ %заточки/100)/3*силу
            var result = (BaseDamage / 7.8f * levelMod) * GetRaretyModifier(character.Level, true) * (1 + Sharpening / 100f) / 3 * Strength;
            return result;
        }

        public float GetMeleeDamage(int level, int sharpening)
        {
            var levelMod = DomainConfiguration.EquipmentLevelPercents[level];
            // УРОН= (базовый урон/7.8*значение уровня)*Мод.редкости(1,1.3,1.6,1.9)*(1+ %заточки/100)
            var result = (BaseDamage / 7.8f * levelMod) * GetRaretyModifier(level, true) * (1f + sharpening / 100f);
            return result;
        }

        public float GetMagicDamage(int level, int sharpening)
        {
            var levelMod = DomainConfiguration.EquipmentLevelPercents[level];

            // УРОН ЗАРЯДА - (базовый урон/7.8*значение уровня)*Мод.редкости(1,1.3,1.6,1.9)*(1+ %заточки/100)/3
            var result = (BaseDamage / 7.8f * levelMod) * GetRaretyModifier(level, true) * (1 + sharpening / 100f) / 3;
            return result;
        }

        public float GetRangeDamage(int level, int sharpening)
        {
            var levelMod = DomainConfiguration.EquipmentLevelPercents[level];

            // УРОН ВЫСТРЕЛА – (базовый урон/7.8*значение уровня)*Мод.редкости(1,1.3,1.6,1.9)*(1+ %заточки/100)/3*силу
            var result = (BaseDamage / 7.8 * levelMod) * GetRaretyModifier(level, true) * (1 + sharpening / 100f) / 3 * Strength;
            return (float)result;
        }

        public float GetTotalAnomality(Character character)
        {
            var result = Anomality * GetRaretyModifier(character.Level, false) * (1f + Sharpening / 100f); 
            return result;
        }

        public EnchatInfoResult CalcucaleAndGetEnchantInfo(Character character,
            BaseElement element, int curElementCount)
        {
            var damage = 0f;
            if (WeaponType == WeaponTypes.Magic)
            {
                damage = GetMagicDamage(character);
            }
            if(WeaponType == WeaponTypes.Ranged)
            {
                damage = GetRangeDamage(character);
            }
            if(WeaponType == WeaponTypes.Melee)
            {
                damage = GetMeleeDamage(character);
            }

            var info = new EnchatInfoResult
            {
                CurrentAttack = damage,
                CurrentEnchatPercent = Sharpening
            };

            var priceTable = new float[51];
            var prevPrice = Price/10f;
            priceTable[0] = 0;
            priceTable[1] = prevPrice;
            prevPrice = priceTable[1];

            for (var i = 2; i < 51; i++)
            {
                priceTable[i] = prevPrice*1.05f;
                prevPrice = priceTable[i];
            }

            var lastTotatSum = 0f;
            var avaliableSum = element == null ? 0 : element.Price*curElementCount;
            avaliableSum = avaliableSum > 0 ? avaliableSum : -1;
            info.NewEnchatPercent = info.CurrentEnchatPercent;
            var startN = Sharpening > 0 ? Sharpening : 1;

            for (var i = startN; i < 51; i++)
            {
                lastTotatSum += priceTable[i];
                if (lastTotatSum <= avaliableSum)
                {
                    info.NewEnchatPercent++;
                }
            }

            if (info.NewEnchatPercent > 50) info.NewEnchatPercent = 50;

            info.ElemetsCountForMaxEcnhat = element == null
                ? 0
                : (lastTotatSum/element.Price).CeilToInt();

            var craftDelta = info.NewEnchatPercent - character.CraftLevel;
            info.ChanceToBroke = (2f*craftDelta*(1f - (craftDelta + character.CraftLevel/2f)/100f)).CeilToInt();

            if (WeaponType == WeaponTypes.Magic)
            {
                damage = GetMagicDamage(character.Level, info.NewEnchatPercent);
            }
            if (WeaponType == WeaponTypes.Ranged)
            {
                damage = GetRangeDamage(character.Level, info.NewEnchatPercent);
            }
            if (WeaponType == WeaponTypes.Melee)
            {
                damage = GetMeleeDamage(character.Level, info.NewEnchatPercent);
            }

            info.NewAttack = damage;
            return info;
        }

        public bool TryEnchant(float crashChance)
        {
            var randomValue = DomainConfiguration.Random.Next(0, 100);
            if (randomValue >= 0 && randomValue <= crashChance)
            {
                return false;
            }
            return true;
        }

        public float GetSharpeningPrice(int sharpeningValue, int craftLevel = 0)
        {
            var price = 0f;
            for (var i = 1; i <= sharpeningValue; i++)
            {
                price = GetSharpeningPricePrivate(i, price);
            }
            return price;
        }

        private float GetSharpeningPricePrivate(int value, float currentSharpeningPrice = 0)
        {
            float price;
            if (value == 1)
            {
                price = Price*0.1f*value;
            }
            else
            {
                price = currentSharpeningPrice*1.05f;
            }
            return (float) Math.Round(price);
        }

        public float GetSharpeningTotalPrice()
        {
            var s = 0f;
            for (var i = 1; i <= Sharpening; i++)
            {
                s = s + GetSharpeningPrice(i);
            }
            return s;
        }

        public float GetWeaponDamageByType(Character c)
        {
            switch(WeaponType)
            {
                case WeaponTypes.None:
                    return 0;
                case WeaponTypes.Melee:
                    return GetMeleeDamage(c);
                case WeaponTypes.Ranged:
                    return GetRangeDamage(c);
                case WeaponTypes.Magic:
                    return GetMagicDamage(c);
            }
            return 0;
        }
        #endregion
    }
}