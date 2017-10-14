namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class MMastery : BaseSkill
    {
        public MMastery()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.MasterySection;
        }
    }
}
