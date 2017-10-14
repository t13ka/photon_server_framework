namespace YourGame.Common.Domain.Skills.Secondary.AbilityBreak
{
    using YourGame.Common.Domain.Enums;

    public class AbilityBreak : BaseSkill
    {
        public AbilityBreak()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 50;
            Priority = 150;
        }
    }
}
