namespace YourGame.Common.Domain.Skills.Secondary.Others
{
    using YourGame.Common.Domain.Enums;

    public class CriticalHitPercent : BaseSkill
    {
        public CriticalHitPercent()
        {
            SkillGroupType = SkillGroupTypes.Secondary2;
            SkillSection = SkillSection.MasterySection;
            Set(50);
        }
    }
}
