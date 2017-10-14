namespace YourGame.Common.Domain.Skills.Secondary.Resistence
{
    using YourGame.Common.Domain.Enums;

    public class SpiritResistance : BaseSkill
    {
        public SpiritResistance()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.DefenceSection;
            BpPercent = 12;
            Priority = 100;
        }
    }
}
