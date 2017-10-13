using Warsmiths.Common.Domain.CommonCharacterProfile.Attributes;
using Warsmiths.Common.Domain.CommonCharacterProfile.Groups;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Primary
{
    public class SkillPrimary
    {
        public Atribute<float> Value;

        // Вторички 
        public Atribute<float> Endurance;
        public Atribute<float> Adaptability;

        // Вторички 2 порядка
        public Atribute<float> Autonomy;
        public Atribute<float> EnergyComsuption;
        public Atribute<float> ModulesCapacity;
        public Atribute<float> SkillsCapacity;
        public Atribute<float> TotalSpeed;
        public Atribute<float> TotalAnomaly;
        public Atribute<float> TotalWeight;
        public Atribute<float> Recovery;
        public Atribute<float> Energy;
        public Atribute<float> Reloading;
        public Atribute<float> Buff;
        public Atribute<float> Decay;
        public Atribute<float> PowerStrength;
        public Atribute<float> SpeedStrength;
        public Atribute<float> IntellectStrength;
        public Atribute<float> EnduranceStrength;
        public Atribute<float> DexterityStrength;
        public Atribute<float> WisdomStrength;

        public SkillPrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Primary,
                Description = "",
                Name = "Skill"
            };
            //------------------------------
            Endurance = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Description = "Способность  длительное время выживать во враждебной среде",
                Name = "Endurance"
            };
            Adaptability = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Description = "Способность использовать больше одного модуля с одинаковым бонусом",
                Name = "Adaptability"
            };

            //------------------------------
            Autonomy = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Время, по истечению которого, на чужой территории начнется истощение",
                Name = "Autonomy"
            };

            EnergyComsuption = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Потребление энергии",
                Name = "Energy Comsuption"
            };

            ModulesCapacity = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Потребление энергетических резервов космической брони модулями",
                Name = "Modules Capacity"
            };
            SkillsCapacity = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Потребление биологических резервов доспеха использованием навыков",
                Name = "Skills Capacity"
            };
            TotalSpeed = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Средняя скорость всех предметов снаряжения ",
                Name = "Total Speed"
            };
            TotalAnomaly = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Средняя аномальность всех предметов снаряжения ",
                Name = "Total Anomaly"
            };
            TotalWeight = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Суммарный вес всего снаряжения",
                Name = "Total Weight"
            };
            Recovery = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Скорость с которой абилки вновь становиться доступны после использования",
                Name = "Recovery"
            };
            Energy = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Затраты энергии ярости или морали на использование абилок",
                Name = "Energy"
            };
            Reloading = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Скорость перезарядки магического и дальнего оружия",
                Name = "Reloading"
            };
            Buff = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Длительность действия бафов",
                Name = "Buff"
            };
            Decay = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Скорость сокращения ярости и морали в отсутствии активного приема",
                Name = "Decay"
            };

            PowerStrength = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "",
                Name = "Power Strength"
            };
            SpeedStrength = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "",
                Name = "Speed Strength"
            };
            EnduranceStrength = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "",
                Name = "Endurance Strength"
            };
            IntellectStrength = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "",
                Name = "Intellect Strength"
            };
            WisdomStrength = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "",
                Name = "Wisdom Strength"
            };
            DexterityStrength = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "",
                Name = "Dexterity Strength"
            };
        }

        public void Calculate(Character c)
        {
            Value.Value = 0;
            Value.BpPercent = 0;

            Adaptability.Value = CommonCharacterProfile.CommonAbv[c.Level] *
                             MasteryGroup.ModBossPoint[c.CommonProfile.AdaptabilityBossPoints];
            Adaptability.BpPercent = Adaptability.Value / 50f;
            
            Endurance.Value = CommonCharacterProfile.CommonAbv[c.Level] *
                             MasteryGroup.ModBossPoint[c.CommonProfile.EnduranceBossPoints];
            Endurance.BpPercent = Endurance.Value / 50f;

            SkillsCapacity.Value = c.CommonProfile.Characteristics.Dexterity.Percent + 
                                   c.CommonProfile.Characteristics.Endurance.Percent +
                                   c.CommonProfile.Characteristics.Intellect.Percent +
                                   c.CommonProfile.Characteristics.Power.Percent +
                                   c.CommonProfile.Characteristics.Speed.Percent +
                                   c.CommonProfile.Characteristics.Wisdom.Percent;
            SkillsCapacity.BpPercent = 0;

            ModulesCapacity.Value = c.Equipment.GetModulesCapacity(); 
            ModulesCapacity.BpPercent = 0;

            EnergyComsuption.Value = 50 + ModulesCapacity.Value + SkillsCapacity.Value;

            Autonomy.Value = Endurance.Value/EnergyComsuption.Value;
            // TODO: CALC
            Recovery.Value = 0;
            Recovery.BpPercent = 0;

            // TODO: CALC
            Energy.Value = 0;
            Energy.BpPercent = 0;

            // TODO: CALC
            Reloading.Value = 0;
            Reloading.BpPercent = 0;

            // TODO: CALC
            Buff.Value = 0;
            Buff.BpPercent = 0;

            // TODO: CALC
            Decay.Value = 0;
            Decay.BpPercent = 0;

            TotalAnomaly.Value = c.Equipment.TotalAnomality();
            TotalWeight.Value = c.Equipment.TotalWeight();
            TotalAnomaly.BpPercent = 0;
            TotalWeight.Value = c.Equipment.TotalWeight();
            TotalWeight.BpPercent = 0;

            PowerStrength.Value = c.CommonProfile.Characteristics.Power.Strenght;
            SpeedStrength.Value = c.CommonProfile.Characteristics.Speed.Strenght;
            IntellectStrength.Value = c.CommonProfile.Characteristics.Intellect.Strenght;
            EnduranceStrength.Value = c.CommonProfile.Characteristics.Endurance.Strenght;
            DexterityStrength.Value = c.CommonProfile.Characteristics.Dexterity.Strenght;
            WisdomStrength.Value = c.CommonProfile.Characteristics.Wisdom.Strenght;
        }
    }
}
