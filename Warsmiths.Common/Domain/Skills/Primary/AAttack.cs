namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class AAttack : BaseSkill
    {
        public AAttack()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.AttackSection;
        }
    }
}
