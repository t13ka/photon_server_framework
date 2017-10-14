namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class MMovement : BaseSkill
    {
        public MMovement()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.MasterySection;
        }
    }
}
