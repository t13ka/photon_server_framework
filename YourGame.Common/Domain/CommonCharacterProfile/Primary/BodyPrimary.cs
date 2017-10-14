namespace YourGame.Common.Domain.CommonCharacterProfile.Primary
{
    using YourGame.Common.Domain.CommonCharacterProfile.Attributes;
    using YourGame.Common.Domain.CommonCharacterProfile.Groups;
    using YourGame.Common.Domain.Enums;

    public class BodyPrimary
    {
        public Atribute<float> Value;

        // Вторички
        public Atribute<float> Improvements;

        // Вторички 2 порядка
        public Atribute<float> Movement;
        public Atribute<float> Charge;
        public Atribute<float> Vision;

        public BodyPrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Primary,
                Description = "",
                Name = "Body"
            };
            //------------------------------
            Improvements = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Description = "Способность использовать апгрейды брони для эффективного перемещения",
                Name = "Control"
            };
            //------------------------------------
            Movement = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Максимальная скорость передвижения",
                Name = "Movement"
            };
            Charge = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Способность пробегать насквозь расталкивая менее тяжелых противников",
                Name = "Charge"
            };
            Vision = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "Определяет радиус зоны обзора игрока",
                Name = "Vision"
            };
        }

        public void Calculate(Character c)
        {
            Value.Value = 0;

            Movement.Value = 0;

            Improvements.Value = CommonCharacterProfile.CommonAbv[c.Level] * MasteryGroup.ModBossPoint[c.CommonProfile.ImprovmentsBossPoints];
            Improvements.BpPercent = Improvements.Value / 50f;

            Charge.Value = c.Equipment.GetCharger(c.CommonProfile, WeaponTypes.Magic).RightHandValue;
            Vision.Value = 0;
        }
    }
}
