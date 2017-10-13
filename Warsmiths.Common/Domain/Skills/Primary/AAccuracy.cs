using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Primary
{
    public class AAccuracy : BaseSkill
    {
        public AAccuracy()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.AttackSection;
        }
    }
}
