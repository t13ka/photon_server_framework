using System;
using System.Collections.Generic;

namespace YourGame.Server.Factories
{
    using YourGame.Common.Domain;
    using YourGame.Common.Domain.Enums;
    using YourGame.Common.Domain.Equipment;

    public static class EquipmentFactory
    {
        public static BaseArmor CreateArmor(BaseReciept r)
        {
            var armor = new BaseArmor();
            if (r.ArmorType == ArmorTypes.Heavy)
            {
                armor = new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    ArmorType = ArmorTypes.Heavy,
                    Weight = 900,
                    Name = "Heavy Armor",
                };
            }
            if (r.ArmorType == ArmorTypes.Medium)
            {
                armor = new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    Weight = 500,
                    ArmorType = ArmorTypes.Medium,
                    Name = "Medium Armor",
                };
            }
            if(r.ArmorType == ArmorTypes.Light)
            {
                armor = new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    Weight = 500,
                    ArmorType = ArmorTypes.Light,
                    Name = "Light Armor",
                };
            }
            armor._id = Guid.NewGuid().ToString();
            armor.Rarety = r.Rarity;
            armor.RaceType = r.Race;
            armor.ArmorType = r.ArmorType;
            armor.ArmorParts = r.PartsPosition;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < armor.ArmorParts.Count; i++)
            {
                if (armor.ArmorParts[i].Modules == null)
                    armor.ArmorParts[i].Modules = new List<BaseModule>();
            }
            armor.FireShield = r.Red;
            armor.ElectricityShield = r.Yellow;
            armor.IceShield = r.Blue;
            armor.Price = r.Price;
            armor.Weight = r.Weight;
            armor.Anomality = r.Anomality;
            armor.RecieptId = r._id;
            armor.Triggers = r.Triggers ?? new List<TriggerTypes>();
            armor.OverallTriggers = armor.Triggers.Count;

            return armor;
        }

        public static BaseArmor CreateArmor(ArmorTypes armorType, RaceTypes raceType)
        {
            var armor = new BaseArmor();
            if (armorType == ArmorTypes.Heavy)
            {
                armor = new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    ArmorType = ArmorTypes.Heavy,
                    Weight = 900,
                    Name = "Heavy Armor"
                };
            }
            if (armorType == ArmorTypes.Medium)
            {
                armor = new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    Weight = 500,
                    ArmorType = ArmorTypes.Medium,
                    Name = "Medium Armor"
                };
            }
            if (armorType == ArmorTypes.Light)
            {
                armor = new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    Weight = 500,
                    ArmorType = ArmorTypes.Light,
                    Name = "Light Armor"
                };
            }
            armor._id = Guid.NewGuid().ToString();
            armor.Rarety = RaretyTypes.Regular;
            armor.RaceType = raceType;
            armor.ArmorType = armorType;
            armor.InitializeDefaultArmorParts();
            armor.ElectricityShield = armor.Durability;
            armor.FireShield = armor.Durability;
            armor.IceShield = armor.Durability;
            armor.Triggers = new List<TriggerTypes>();
            armor.OverallTriggers = armor.Triggers.Count;
            return armor;
        }
    }
}