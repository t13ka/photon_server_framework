namespace YourGame.Common.Domain.Skills.Secondary.AbilityBreak
{
    using YourGame.Common.Domain.Enums;

    public class SpiritBreak : BaseSkill
    {
        public SpiritBreak()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 8;
            Priority = 150;
        }
    }
}