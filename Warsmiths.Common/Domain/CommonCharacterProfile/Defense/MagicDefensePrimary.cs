using Warsmiths.Common.Domain.CommonCharacterProfile.Attributes;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Defense
{
    public class MagicDefensePrimary
    {
        public Atribute<float> Value;

        // Вторички
        public Atribute<float> SpiritResist;
        public Atribute<float> Armor;
        public Atribute<float> ResistanceMod;
        public Atribute<string> SpecialDefense;
        public Atribute<RaretyTypes> Rarity;
        public Atribute<float> Anomaly;
        public Atribute<float> Weight;

        public MagicDefensePrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Primary,
                Name = "Magic Defense",
                Description = ""
            };
            //---------------------------------
            SpiritResist = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Name = "Spirit Resist",
                Description = "Способность противостоять понижению защиты и атаки"
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
            SpecialDefense = new Atribute<string>
            {
                Value = "",
                Type = SkillGroupTypes.Secondary2,
                Name = "Special Defence",
                Description = "Тактическая способность, активируемая через определенное время щитом"
            };

            Rarity = new Atribute<RaretyTypes>
            {
                Value = RaretyTypes.Regular,
                Type = SkillGroupTypes.Secondary2,
                Name = "Rarity",
                Description = "Редкость щита определяется качеством ковки"
            };
            Anomaly = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Anomaly",
                Description = "Аномальность щита повышает обзор и снижает стойкость"
            };
            Weight = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Weight",
                Description = "Вес щита повышает чардж и снижает скорость передвижения "
            };
        }

        public void Calculate(Character c)
        {
            Value.Value = 0;
            Value.BpPercent = 0;


            var armor = c.Equipment.Armor;
            if (armor != null)
            {
                ResistanceMod.Value = (armor.ArmorType == ArmorTypes.Heavy ? 1.4f : 1);
                SpiritResist.Value = CommonCharacterProfile.CommonAbv[c.Level]* ResistanceMod.Value * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MagicDefenceSpiritResist); 
                SpiritResist.BpPercent = 0;
            }
            var lh = c.Equipment.LeftWeapon;
            if (lh != null && lh.WeaponSize == WeaponSizeTypes.Shield && lh.WeaponType == WeaponTypes.Magic)
            {
                SpecialDefense.Value = lh.Name;
                Armor.Value = lh.BaseDamage;
                Rarity.Value = lh.Rarety;
                Anomaly.Value = lh.GetTotalAnomality(c);
                Weight.Value = lh.Weight;   
            }
            else
            {
                SpecialDefense.Value = "";
                Armor.Value = 0;
                Rarity.Value = RaretyTypes.Regular;
                Anomaly.Value = 0;
                Weight.Value = 0;
            }
        }
    }
}
