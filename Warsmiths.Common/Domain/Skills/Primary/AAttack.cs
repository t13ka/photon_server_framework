using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Primary
{
    public class AAttack : BaseSkill
    {
        public AAttack()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.AttackSection;
        }
    }
}
