namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class APenetration : BaseSkill
    {
        public APenetration()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.AttackSection;
        }
    }
}
