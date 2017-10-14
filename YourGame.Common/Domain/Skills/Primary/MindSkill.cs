namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class MMind : BaseSkill
    {
        public MMind()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.MasterySection;
        }
    }
}
