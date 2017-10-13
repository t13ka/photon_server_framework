using Warsmiths.Common.Domain.CommonCharacterProfile.Attributes;
using Warsmiths.Common.Domain.CommonCharacterProfile.Groups;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Primary
{
    public class MindPrimary
    {
        public Atribute<float> Value;

        // ¬торички
        public Atribute<float> Willpower;

        // ¬торички 2 пор€дка
        public Atribute<float> Initiative;
        public Atribute<float> Immunity;
        public Atribute<float> Stamina;

        public MindPrimary()
        {
            Value = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Primary,
                Description = "",
                Name = "Mind"
            };
            //------------------------------------
            Willpower = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary1,
                Description = "—пособность подавл€ть аномальность и использовать свойства брони",
                Name = "Willpower"
            };
            //------------------------------------
            Initiative = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "—пособность противосто€ть специальным атакам оружи€ противника",
                Name = "Reaction"
            };
            Immunity = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "ѕри равенстве бросков кубика в формулах  делает выбор в пользу игрока",
                Name = "Initiative"
            };
            Stamina = new Atribute<float>
            {
                Value = 0,
                Type = SkillGroupTypes.Secondary2,
                Description = "—пособность игрока защищатьс€ от большого числа одновременных атак",
                Name = "Stamina"
            };
        }

        public void Calculate(Character c)
        {
            Value.Value = 0;
            Value.BpPercent = 0;

            Willpower.Value = CommonCharacterProfile.CommonAbv[c.Level]*
                              MasteryGroup.ModBossPoint[c.CommonProfile.WillpowerBossPoints];
            Willpower.BpPercent = Willpower.Value/50f;

            Initiative.Value = 0;
            Initiative.BpPercent = 0;

            Immunity.Value = 0;
            Immunity.BpPercent = 0;

            Stamina.Value = 0;
            Stamina.BpPercent = 0;
        }
    }
}
