using System;
using System.Threading;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Enums.ItemGeneration;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Domain.VictoryPrizes;
using Warsmiths.Server.MasterServer;
// ReSharper disable SwitchStatementMissingSomeCases

namespace Warsmiths.Server.VictoryPrizes
{
    public class VictoryPrizesGenerator
    {
        public static BaseArmor CreateArmorForGenerator(ArmorTypes aType, RaretyTypes rare, int prok)
        {
            var durabilty = 0;
            var modulesCount = 0;
            var casing = 0;
            var anomality = 0;
            var weight = 0;
            var price = 0;
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (aType == ArmorTypes.Light)
            {
                switch (rare)
                {
                    case RaretyTypes.Regular:
                        durabilty = 220;
                        modulesCount = 1;
                        casing = 20;
                        anomality = 10;
                        weight = 250;
                        price = 3600;
                        break;
                    case RaretyTypes.Rare:
                        durabilty = 300;
                        modulesCount = 1;
                        casing = 30;
                        anomality = 20;
                        weight = 220;
                        price = 5000;
                        break;
                    case RaretyTypes.Epic:
                        durabilty = 400;
                        modulesCount = 1;
                        casing = 40;
                        anomality = 30;
                        weight = 200;
                        price = 6500;
                        break;
                    case RaretyTypes.Legend:
                        durabilty = 500;
                        modulesCount = 2;
                        casing = 50;
                        anomality = 40;
                        weight = 180;
                        price = 9000;
                        break;
                }
            }
            else if (aType == ArmorTypes.Medium)
            {
                switch (rare)
                {
                    case RaretyTypes.Regular:
                        durabilty = 400;
                        modulesCount = 2;
                        casing = 20;
                        anomality = 15;
                        weight = 500;
                        price = 5000;
                        break;
                    case RaretyTypes.Rare:
                        durabilty = 500;
                        modulesCount = 2;
                        casing = 30;
                        anomality = 25;
                        weight = 460;
                        price = 6500;
                        break;
                    case RaretyTypes.Epic:
                        durabilty = 650;
                        modulesCount = 2;
                        casing = 40;
                        anomality = 35;
                        weight = 420;
                        price = 9000;
                        break;
                    case RaretyTypes.Legend:
                        durabilty = 800;
                        modulesCount = 3;
                        casing = 50;
                        anomality = 45;
                        weight = 380;
                        price = 11000;
                        break;
                }
            }
            else
            {
                switch (rare)
                {
                    case RaretyTypes.Regular:
                        durabilty = 600;
                        modulesCount = 3;
                        casing = 20;
                        anomality = 20;
                        weight = 1000;
                        price = 6500;
                        break;
                    case RaretyTypes.Rare:
                        durabilty = 750;
                        modulesCount = 3;
                        casing = 30;
                        anomality = 30;
                        weight = 920;
                        price = 9000;
                        break;
                    case RaretyTypes.Epic:
                        durabilty = 1100;
                        modulesCount = 3;
                        casing = 40;
                        anomality = 40;
                        weight = 840;
                        price = 11000;
                        break;
                    case RaretyTypes.Legend:
                        durabilty = 1300;
                        modulesCount = 4;
                        casing = 50;
                        anomality = 50;
                        weight = 760;
                        price = 15000;
                        break;
                }
            }

            durabilty = (int)Math.Round(durabilty * (1f - prok / 100f));
            weight = (int)Math.Round(weight * (1f - prok / 100f));

            var armor = new BaseArmor
            {
                Rarety = rare,
                Anomality = anomality,
                Weight = weight,
                ArmorType = aType,
                Price = price,
                Prok = prok
            };

            armor.InitializeArmorParts(durabilty, modulesCount, casing);
            return armor;

        }
        public static BaseArmor GetArmorTypeByLuck(ItemGenerationLuckTypes luck, RaretyTypes armorRare, int prok)
        {
            var armor = CreateArmorForGenerator(ArmorTypes.Light, RaretyTypes.Regular, prok);
            int addRan;
            switch (luck)
            {
                case ItemGenerationLuckTypes.Regular:
                    armor = CreateArmorForGenerator(ArmorTypes.Light, armorRare, prok);
                    armor.StructureType = (StructureTypes)DomainConfiguration.Random.Next(0, (int)StructureTypes.Crystal + 1);
                    break;

                case ItemGenerationLuckTypes.Rare:
                    var r = DomainConfiguration.Random.Next(0, 2);
                    armor = r == 0 ? CreateArmorForGenerator(ArmorTypes.Light, armorRare, prok) :
                        CreateArmorForGenerator(ArmorTypes.Medium, armorRare, prok);

                    armor.StructureType = (StructureTypes)DomainConfiguration.Random.Next(0, (int)StructureTypes.CrystoPlastic + 1);
                    break;

                case ItemGenerationLuckTypes.Epic:
                    addRan = DomainConfiguration.Random.Next(0, 3);
                    switch (addRan)
                    {
                        case 0:
                            armor = CreateArmorForGenerator(ArmorTypes.Light, armorRare, prok);
                            break;
                        case 1:
                            armor = CreateArmorForGenerator(ArmorTypes.Medium, armorRare, prok);
                            break;
                        default:
                            armor = CreateArmorForGenerator(ArmorTypes.Heavy, armorRare, prok);
                            break;
                    }
                    armor.StructureType = (StructureTypes)DomainConfiguration.Random.Next(0, (int)StructureTypes.CrystoPlastic + 1);
                    break;

                case ItemGenerationLuckTypes.Legend:
                    addRan = DomainConfiguration.Random.Next(0, 3);
                    switch (addRan)
                    {
                        case 0:
                            armor = CreateArmorForGenerator(ArmorTypes.Light, armorRare, prok);
                            break;
                        case 1:
                            armor = CreateArmorForGenerator(ArmorTypes.Medium, armorRare, prok);
                            break;
                        default:
                            armor = CreateArmorForGenerator(ArmorTypes.Heavy, armorRare, prok);
                            break;
                    }
                    armor.StructureType = (StructureTypes)DomainConfiguration.Random.Next(0, (int)StructureTypes.Light + 1);
                    break;
            }

            return armor;
        }

        private static VictoryPrize GenerateEquipment(ItemGenerationMasteryTypes mastery, ItemGenerationLuckTypes luck,
            ItemGenerationMutualAidTypes mutualAid)
        {
            Thread.Sleep(1);
            var itemRarity = AdjustItemRarety(mastery);
            // randomize the prok
            var prok = DomainConfiguration.Random.Next(0, 22);
            var prize = GenerateSpecificTypeOfItem(luck, mutualAid, itemRarity, prok);

            // randomize the color 
            

            var weapon = prize.Item as BaseWeapon;
            var armor = prize.Item as BaseArmor;
            var module = prize.Item as BaseModule;

            if (weapon != null)
            {
                weapon.Anomality = weapon.Anomality - (weapon.Anomality * weapon.Prok) / 100;
                // увеличим базовый урон в зависимости от редкости
                weapon.BaseDamage = weapon.BaseDamage - (weapon.BaseDamage * weapon.Prok) / 100;
                switch (weapon.Rarety)
                {
                    case RaretyTypes.Regular:
                        break;
                    case RaretyTypes.Rare:
                        weapon.BaseDamage += weapon.BaseDamage * 0.3f;
                        break;
                    case RaretyTypes.Epic:
                        weapon.BaseDamage += weapon.BaseDamage * 0.6f;
                        break;
                    case RaretyTypes.Legend:
                        weapon.BaseDamage += weapon.BaseDamage * 0.9f;
                        break;
                }

                // увеличим аномальности в зависимости от редкости
                switch (weapon.Rarety)
                {
                    case RaretyTypes.Regular:
                        break;
                    case RaretyTypes.Rare:
                        weapon.Anomality += weapon.Anomality * 0.3f;
                        break;
                    case RaretyTypes.Epic:
                        weapon.Anomality += weapon.Anomality * 0.6f;
                        break;
                    case RaretyTypes.Legend:
                        weapon.Anomality += weapon.Anomality * 0.9f;
                        break;
                }

                // add bonus to legend item
                if (weapon.Rarety == RaretyTypes.Legend)
                {
                    // TODO: как то добавить в куда-то модуль
                    /*var epicModule = (BaseModule)GetRandomEq<BaseModule>(chanceGroup);
                    epicModule.RandomizeImpactProperty(luck);
                    epicModule.Value = 10;
                    epicModule.Rarety = RaretyTypes.Epic;*/
                    //weapon.SpecialAttack = "with module(" + epicModule + ")"; .. 
                    //weapon.BaseDamage = weapon.BaseDamage + weapon.BaseDamage * 0.1f;
                }
            }


            if (module != null)
            {
                module.RandomizeImpactProperty(luck);
                module.InsertTo = module.GetRandomAvaliablePlace();
                var mod = 0;
                switch (module.Group)
                {
                    case ItemGenerationLuckTypes.Rare:
                        mod = 1;
                        break;
                    case ItemGenerationLuckTypes.Epic:
                        mod = 2;
                        break;
                    case ItemGenerationLuckTypes.Legend:
                        mod = 4;
                        break;
                }
                module.Price *= 1 + 0.1f * mod;
            }

            // Меняем цены
            if (prize.Type == VictoryPrizeTypeE.Equipment && prize.Item != null)
            {
                ((BaseEquipment)prize.Item).Price *= 1 - prok/100f;

                if (armor == null)
                {
                    var mod = 1;
                    switch (itemRarity)
                    {
                        case RaretyTypes.Rare:
                            mod = 5;
                            break;
                        case RaretyTypes.Epic:
                            mod = 10;
                            break;
                        case RaretyTypes.Legend:
                            mod = 20;
                            break;
                    }
                    ((BaseEquipment)prize.Item).Price *= mod;
                }
            }

            return prize;
        }

        private static RaretyTypes AdjustItemRarety(ItemGenerationMasteryTypes mastery)
        {
            var chance = DomainConfiguration.Random.Next(0, 100);
            switch (mastery)
            {
                case ItemGenerationMasteryTypes.Regular:
                    return chance < 90 ? RaretyTypes.Regular : RaretyTypes.Rare;

                case ItemGenerationMasteryTypes.Rare:
                    if (chance < 25)
                    {
                        return RaretyTypes.Regular;
                    }
                    return chance < 90 ? RaretyTypes.Rare : RaretyTypes.Epic;

                case ItemGenerationMasteryTypes.Epic:
                    if (chance < 25)
                    {
                        return RaretyTypes.Rare;
                    }
                    return chance < 90 ? RaretyTypes.Epic : RaretyTypes.Legend;

                case ItemGenerationMasteryTypes.Legend:
                    return chance < 40 ? RaretyTypes.Epic : RaretyTypes.Legend;

                default:
                    return RaretyTypes.Regular;
            }
        }

        private static VictoryPrize GenerateSpecificTypeOfItem(ItemGenerationLuckTypes luck, ItemGenerationMutualAidTypes mutualAid, RaretyTypes rarity, int prok)
        {
            var prize = new VictoryPrize();
            BaseEquipment item = null;
            var chance = DomainConfiguration.Random.Next(0, 100);
            var rnd = DomainConfiguration.Random.Next(0, 100);
            var chanceGroupByLuck = ItemGenerationLuckTypes.Regular;
            switch (luck)
            {
                case ItemGenerationLuckTypes.Regular:
                    chanceGroupByLuck = rnd < 90 ? ItemGenerationLuckTypes.Regular : ItemGenerationLuckTypes.Rare;
                    break;
                case ItemGenerationLuckTypes.Rare:
                    if (rnd < 25)
                    {
                        chanceGroupByLuck = ItemGenerationLuckTypes.Regular;
                    }
                    else
                    {
                        chanceGroupByLuck = rnd < 90 ? ItemGenerationLuckTypes.Rare : ItemGenerationLuckTypes.Epic;
                    }
                    break;
                case ItemGenerationLuckTypes.Epic:
                    if (rnd < 25)
                    {
                        chanceGroupByLuck = ItemGenerationLuckTypes.Rare;
                    }
                    else
                    {
                        chanceGroupByLuck = (rnd < 90) ? ItemGenerationLuckTypes.Epic : ItemGenerationLuckTypes.Legend;
                    }
                    break;
                case ItemGenerationLuckTypes.Legend:
                    chanceGroupByLuck = rnd < 40 ? ItemGenerationLuckTypes.Epic : ItemGenerationLuckTypes.Legend;
                    break;
            }

            var chanceGroup = (int)chanceGroupByLuck + 1;
            switch (mutualAid)
            {
                case ItemGenerationMutualAidTypes.Regular:
                    item = chance < 90 ? new BaseModule() : MasterApplication.DomainConfiguration.GetRandomEq<BaseWeapon>(chanceGroup);
                    break;

                case ItemGenerationMutualAidTypes.Rare:
                    if (chance < 25)
                    {
                        item = new BaseModule();
                    }
                    else if (chance < 90)
                    {
                        item = MasterApplication.DomainConfiguration.GetRandomEq<BaseWeapon>(chanceGroup);
                    }
                    else
                    {
                        item = GetArmorTypeByLuck(luck, rarity, prok);
                    }
                    break;

                case ItemGenerationMutualAidTypes.Epic:
                    if (chance < 25)
                    {
                        item = MasterApplication.DomainConfiguration.GetRandomEq<BaseWeapon>(chanceGroup);
                    }
                    else if (chance < 90)
                    {
                        item = GetArmorTypeByLuck(luck, rarity, prok);
                    }
                    else
                    {
                        prize.Type = DomainConfiguration.Random.Next(10) < 8 ? VictoryPrizeTypeE.Crystals : VictoryPrizeTypeE.Keys;
                        prize.CrystalAmout = prize.Type == VictoryPrizeTypeE.Crystals ? 5 : 1;
                    }
                    break;

                case ItemGenerationMutualAidTypes.Legend:
                    if (chance < 40)
                    {
                        item = GetArmorTypeByLuck(luck, rarity, prok);
                    }
                    else if (chance < 100)
                    {
                        prize.Type = DomainConfiguration.Random.Next(10) < 8 ? VictoryPrizeTypeE.Crystals : VictoryPrizeTypeE.Keys;
                        prize.CrystalAmout = prize.Type == VictoryPrizeTypeE.Crystals ? 5 : 1;
                    }
                    break;
            }

            if (item != null)
            {
                var pItem = item.Clone();
                pItem.ColorType =
                    (ElementColorTypes)
                    DomainConfiguration.Random.Next(0, Enum.GetValues(typeof(ElementColorTypes)).Length);

                // randomize the energy
                if (rarity > RaretyTypes.Regular)
                {
                    pItem.EnergyTypes =
                        (EnergyTypes) DomainConfiguration.Random.Next(1, Enum.GetValues(typeof(EnergyTypes)).Length);
                }

                if (!(pItem is BaseModule))
                {
                    pItem.Price = pItem.Price*(1f - prok/100f);
                }
                pItem.Rarety = rarity;
                pItem.Prok = prok;
                prize.Item = pItem;
            }
            return prize;
        }

        public static VictoryPrizesResult GetWinPrisesForPlayer(ItemGenerationMasteryTypes mastery, ItemGenerationLuckTypes luck, ItemGenerationMutualAidTypes mutualAid)
        {
            var result = new VictoryPrizesResult();
            var capturedTerrasCount = DomainConfiguration.Random.Next(1, 10);

            result.Mastery = mastery;
            result.Luck = luck;
            result.MutaulAid = mutualAid;

            result.FarmedCombatPotential = 0;
            for (var i = 0; i < capturedTerrasCount; ++i)
            {
                result.FarmedCombatPotential += DomainConfiguration.Random.Next(700, 1100);
            }

            result.GoldCoinsBonus = DomainConfiguration.Random.Next(-10, 11);
            result.BlackCoinsBonus = DomainConfiguration.Random.Next(-10, 11);

            if (capturedTerrasCount > 4) capturedTerrasCount = 4;

            for (var i = 0; i < capturedTerrasCount * 2; ++i)
            {
                var newp = GenerateEquipment(mastery, luck, mutualAid);
                result.Items.Add(newp);
            }

            for (var i = 0; i < result.Items.Count; i++)
            {
                result.Items[i].PrizeId = i;
            }
            return result;
        }
    }
}
