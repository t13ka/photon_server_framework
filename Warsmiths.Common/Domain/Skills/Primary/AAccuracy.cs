namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class AAccuracy : BaseSkill
    {
        public AAccuracy()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.AttackSection;
        }
    }
}
