namespace YourGame.Common.Domain.Skills.Secondary.Others
{
    using YourGame.Common.Domain.Enums;

    public class CriticalHitStrength : BaseSkill
    {
        public CriticalHitStrength()
        {
            SkillGroupType = SkillGroupTypes.Secondary2;
            SkillSection = SkillSection.MasterySection;
            Set(30);
        }
    }
}
