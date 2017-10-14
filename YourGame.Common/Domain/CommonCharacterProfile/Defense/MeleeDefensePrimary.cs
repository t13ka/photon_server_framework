namespace YourGame.Common.Domain.CommonCharacterProfile.Defense
{
    using System;
    using System.Linq;

    using YourGame.Common.Domain.CommonCharacterProfile.Attributes;
    using YourGame.Common.Domain.Enums;

    public class MeleeDefensePrimary
    {
        public Atribute<float> Value;
        // Вторички
        public Atribute<float> TotalDurability;
        public Atribute<float> AbilityResist;
        // Вторички 2 порядка
        public Atribute<float> Hitpoints;
        public Atribute4<float> Chest;
        public Atribute4<float> Back;
        public Atribute4<float> RightHand;
        public Atribute4<float> LeftHand;
        public Atribute4<float> LeftLeg;
        public Atribute4<float> RightLeg;
        public Atribute<float> FireShield;
        public Atribute<float> IceShield;
        public Atribute<float> ElectricityShield;
        public Atribute<float> Armor;
        public Atribute<float> ResistanceMod;
        public Atribute<float> AttackAngle;
        public Atribute<ArmorTypes> Type;
        public Atribute<StructureTypes> Structure;
        public Atribute<float> GeneralCasing;
        public Atribute<float> OverallTriggers;
        public Atribute<float> ActiveTriggers;
        public Atribute<float> OverallUpgrades;
        public Atribute<float> ActiveUpgrades;
        public Atribute<float> OverallModules;
        public Atribute<float> ActiveModules;
        public Atribute<string> SpecialDefense;
        public Atribute2<RaretyTypes> Rarity;
        public Atribute2<float> Anomaly;
        public Atribute2<float> Weight;

        public MeleeDefensePrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Primary,
                Name = "Melee Defense",
                Description = ""
            };
            //---------------------------------
            TotalDurability = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Name = "Total Durability",
                Description = "Определяет максимально количество физического урона, выдерживаемое доспехом"
            };
            AbilityResist = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Name = "Ability Resist",
                Description = "Способность противостоять срыву применения накопленных приемов"
            };
            //---------------------------------
            Chest = new Atribute4<float>
            {
                Value1 = 0,
                Value2 = 0,
                Value3 = 0,
                Value4 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Chest",
                Description = "Сокращение прочности груди до нуля приводит к гибели"
            };
            Back = new Atribute4<float>
            {
                Value1 = 0,
                Value2 = 0,
                Value3 = 0,
                Value4 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Back",
                Description = "Сокращение прочности Спины до нуля приводит к гибели "
            };
            RightHand = new Atribute4<float>
            {
                Value1 = 0,
                Value2 = 0,
                Value3 = 0,
                Value4 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Right Hand",
                Description = "Сокращение прочности руки не дает использовать оружие в ней"
            };
            LeftHand = new Atribute4<float>
            {
                Value1 = 0,
                Value2 = 0,
                Value3 = 0,
                Value4 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Left Hand",
                Description = "Сокращение прочности руки не дает использовать оружие в ней"
            };
            LeftLeg = new Atribute4<float>
            {
                Value1 = 0,
                Value2 = 0,
                Value3 = 0,
                Value4 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Left Leg",
                Description = "Сокращение прочности ноги не дает переходить в галоп"
            };
            RightLeg = new Atribute4<float>
            {
                Value1 = 0,
                Value2 = 0,
                Value3 = 0,
                Value4 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Right Leg",
                Description = "Сокращение прочности ноги не дает переходить в галоп"
            };
            FireShield = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Fire Shield",
                Description = "Энерго-прочность доспеха в отношении огня, восстанавливается за 3 мин"
            };
            Hitpoints = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Hitpoints",
                Description = "Броня"
            };
            IceShield = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Ice Shield",
                Description = "Энерго-прочность доспеха в отношении льда, восстанавливается за 3 мин"
            };
            ElectricityShield = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Electricity Shield",
                Description = "Энерго-прочность доспеха в отношении электричества, восстанавливается за 3 мин"
            };
            Armor = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Armor",
                Description = "Процент урона, поглощаемый щитом"
            };

            ResistanceMod = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Resistance Mod",
                Description = "Способность сопротивляться"
            };
            AttackAngle = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Attack Angle",
                Description = "Угол обзора, определяемый броней воспринимаемых противников"
            };
            Type = new Atribute<ArmorTypes>
            {
                Value = ArmorTypes.Medium,
                Type = SkillGroupTypes.Secondary2,
                Name = "Type",
                Description = "Чем больше прочность и вес брони, тем меньше скорость и угол атаки"
            };
            Structure = new Atribute<StructureTypes>
            {
                Value = StructureTypes.Metall,
                Type = SkillGroupTypes.Secondary2,
                Name = "Structure",
                Description = "Преобладающий материал, определяющий характерные свойства Брони"
            };
            GeneralCasing = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "General Casing",
                Description = "Процент прочность при потери, которой модули будут деактивированы"
            };
            OverallTriggers = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Overall Triggers",
                Description = "Общее количество пассивных свойств брони"
            };
            ActiveTriggers = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Active Triggers",
                Description = "Действующие свойства брони за счет подавления аномальности Волей"
            };
            OverallUpgrades = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Overall Upgrades",
                Description = "Общее количество апгрейдов, дающих эффективнее перемещаться"
            };
            ActiveUpgrades = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Active Upgrades",
                Description = "Действующие апгрейды за счет силы Контроля"
            };
            OverallModules = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Overall Modules",
                Description = "Общее количество модулей увеличивающих параметры"
            };
            ActiveModules = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Active Modules",
                Description = "Действующие модули с учетом поврежденных частей доспеха"
            };

            SpecialDefense = new Atribute<string>
            {
                Value = "",
                Type = SkillGroupTypes.Secondary2,
                Name = "Special Defence",
                Description = "Тактическая способность, активируемая через определенное время щитом"
            };

            Rarity = new Atribute2<RaretyTypes>
            {
                Value1 = RaretyTypes.Regular,
                Value2 = RaretyTypes.Regular,
                Type = SkillGroupTypes.Secondary2,
                Name = "Rarity",
                Description = "Редкость брони определяется качеством ковки"
            };
            Anomaly = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Anomaly",
                Description = "Аномальность брони повышает обзор и снижает стойкость"
            };
            Weight = new Atribute2<float>
            {
                Value1 = 0,
                Value2 = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Weight",
                Description = "Вес брони повышает чардж и снижает скорость передвижения "
            };
        }

        public void Calculate(Character c)
        {
            // Primary
            Value.Value = 0;
            Value.BpPercent = 0;

            // Secondary
            var armor = c.Equipment.Armor;
            if (armor != null)
            {
                var def = armor.ArmorParts.Sum(x => x.Durability);
                var levelMod = DomainConfiguration.EquipmentLevelPercents[c.CommonProfile.Level];
                TotalDurability.Value = (def + def / 7f * (1+c.CommonProfile.Characteristics.Dexterity.Percent/100f) * (1 + c.CommonProfile.Characteristics.Dexterity.Strenght/100f)) / 7.8f * levelMod;
                TotalDurability.BpPercent = 0;

                ResistanceMod.Value = (armor.ArmorType == ArmorTypes.Light ? 1.4f : 1);

                AbilityResist.Value = CommonCharacterProfile.CommonAbv[c.Level] * ResistanceMod.Value * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeDefenceAbilityResist);
                AbilityResist.BpPercent = 0;

                Type.Value = armor.ArmorType;
                Structure.Value = armor.StructureType;
                GeneralCasing.Value = armor.Casing * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeDefenceGeneralCasing);

                OverallTriggers.Value = armor.OverallTriggers;
                ActiveTriggers.Value = armor.ActiveTriggers;

                OverallModules.Value = armor.ModulesCount;
                ActiveModules.Value = armor.ActiveModules;
                Rarity.Value1 = armor.Rarety;

                Anomaly.Value1 = armor.Anomality;
                Anomaly.BpPercent = 0;
                Weight.Value1 = armor.Weight;
                Weight.BpPercent = 0;

                OverallUpgrades.Value = armor.OverallUpgrades;
                ActiveUpgrades.Value = armor.ActiveUpgrades;

                // Second Seconadary
                Hitpoints.Value = armor.Durability / 7.8f * levelMod;

                var chest = armor.ArmorParts.FirstOrDefault(t => t.ArmorPartType == ArmorPartTypes.Chest);
                if (chest != null)
                {
                    Chest.Value1 = (int)Math.Ceiling(chest.Durability / 7.8f * levelMod);
                    Chest.Value2 = chest.Casing;
                    Chest.Value3 = chest.MaxModulesCount;
                    Chest.Value4 = chest.Modules.Count;
                }

                var back = armor.ArmorParts.FirstOrDefault(t => t.ArmorPartType == ArmorPartTypes.Back);
                if (back != null)
                {
                    Back.Value1 = (int)Math.Ceiling(back.Durability / 7.8f * levelMod);
                    Back.Value2 = back.Casing;
                    Back.Value3 = back.MaxModulesCount;
                    Back.Value4 = back.Modules.Count;
                }

                var rightHand = armor.ArmorParts.FirstOrDefault(t => t.ArmorPartType == ArmorPartTypes.RightHand);
                if (rightHand != null)
                {
                    RightHand.Value1 = (int)Math.Ceiling(rightHand.Durability / 7.8f * levelMod);
                    RightHand.Value2 = rightHand.Casing;
                    RightHand.Value3 = rightHand.MaxModulesCount;
                    RightHand.Value4 = rightHand.Modules.Count;
                }

                var leftHand = armor.ArmorParts.FirstOrDefault(t => t.ArmorPartType == ArmorPartTypes.LeftHand);
                if (leftHand != null)
                {
                    LeftHand.Value1 = (int)Math.Ceiling(leftHand.Durability / 7.8f * levelMod);
                    LeftHand.Value2 = leftHand.Casing;
                    LeftHand.Value3 = leftHand.MaxModulesCount;
                    LeftHand.Value4 = leftHand.Modules.Count;
                }

                var leftLeg = armor.ArmorParts.FirstOrDefault(t => t.ArmorPartType == ArmorPartTypes.LeftLeg);
                if (leftLeg != null)
                {
                    LeftLeg.Value1 = (int)Math.Ceiling(leftLeg.Durability / 7.8f * levelMod);
                    LeftLeg.Value2 = leftLeg.Casing;
                    LeftLeg.Value3 = leftLeg.MaxModulesCount;
                    LeftLeg.Value4 = leftLeg.Modules.Count;
                }

                var rightLeg = armor.ArmorParts.FirstOrDefault(t => t.ArmorPartType == ArmorPartTypes.RightLeg);
                if (rightLeg != null)
                {
                    RightLeg.Value1 = (int)Math.Ceiling(rightLeg.Durability / 7.8f * levelMod);
                    RightLeg.Value2 = rightLeg.Casing;
                    RightLeg.Value3 = rightLeg.MaxModulesCount;
                    RightLeg.Value4 = rightLeg.Modules.Count;
                }

                
                AttackAngle.Value = armor.AttackAngle * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeDefenceAttackAngle);

                
                var lh = c.Equipment.LeftWeapon;
                if (lh != null && lh.WeaponSize == WeaponSizeTypes.Shield && lh.WeaponType == WeaponTypes.Melee)
                {
                    SpecialDefense.Value = lh.Name;
                    Armor.Value = lh.BaseDamage;

                    Anomaly.Value2 = lh.GetTotalAnomality(c);
                    Weight.Value2 = lh.Weight;
                    Rarity.Value2 = lh.Rarety;
                    Anomaly.UseSecondValue = true;
                    Weight.UseSecondValue = true;
                    Rarity.UseSecondValue = true;
                }
                else
                {
                    SpecialDefense.Value = "";
                    Armor.Value = 0;
                    Rarity.Value2 = RaretyTypes.Regular;
                    Anomaly.Value2 = 0;
                    Weight.Value2 = 0;
                    Anomaly.UseSecondValue = false;
                    Weight.UseSecondValue = false;
                    Rarity.UseSecondValue = false;
                }
            }
        }
    }
}
