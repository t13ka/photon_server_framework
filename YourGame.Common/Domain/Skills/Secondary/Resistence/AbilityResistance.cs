namespace YourGame.Common.Domain.Skills.Secondary.Resistence
{
    using YourGame.Common.Domain.Enums;

    public class AbilityResistance : BaseSkill
    {
        public AbilityResistance()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.DefenceSection;
            BpPercent = 75;
            Priority = 100;
        }
    }
}