using Warsmiths.Common.Domain.CommonCharacterProfile.Attributes;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Defense
{
    public class RangedDefensePrimary
    {
        public Atribute<float> Value;

        // ��������
        public Atribute<float> MovementResist;
        public Atribute<float> Armor;
        public Atribute<string> SpecialDefense;
        public Atribute<RaretyTypes> Rarity;
        public Atribute<float> ResistanceMod;
        public Atribute<float> Anomaly;
        public Atribute<float> Weight;

        public RangedDefensePrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Primary,
                Name = "Ranged Defense",
                Description = ""
            };
            //----------------------------------
            MovementResist = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Name = "Movement Resist",
                Description = "����������� ������������� ���������� ����� � ��������� ��������"
            };

            Armor = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Armor",
                Description = "������� �����, ����������� �����"
            };
            ResistanceMod = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Resistance Mod",
                Description = "����������� ��������������"
            };
            SpecialDefense = new Atribute<string>
            {
                Value = "",
                Type = SkillGroupTypes.Secondary2,
                Name = "Special Defence",
                Description = "����������� �����������, ������������ ����� ������������ ����� �����"
            };

            Rarity = new Atribute<RaretyTypes>
            {
                Value = RaretyTypes.Regular,
                Type = SkillGroupTypes.Secondary2,
                Name = "Rarity",
                Description = "�������� ���� ������������ ��������� �����"
            };
            Anomaly = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Anomaly",
                Description = "������������ ���� �������� ����� � ������� ���������"
            };
            Weight = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Name = "Weight",
                Description = "��� ���� �������� ����� � ������� �������� ������������ "
            };
        }

        public void Calculate(Character c)
        {
            Value.Value = 0;
            Value.BpPercent = 0;

            var armor = c.Equipment.Armor;
            if (armor != null)
            {
                ResistanceMod.Value = (armor.ArmorType == ArmorTypes.Medium ? 1.4f : 1);

                MovementResist.Value = CommonCharacterProfile.CommonAbv[c.Level] * ResistanceMod.Value * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.RangeDefenceMovementResist);  
                MovementResist.BpPercent = 0;

                
                var lh = c.Equipment.LeftWeapon;
                if (lh != null && lh.WeaponSize == WeaponSizeTypes.Shield && lh.WeaponType == WeaponTypes.Ranged)
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
}
