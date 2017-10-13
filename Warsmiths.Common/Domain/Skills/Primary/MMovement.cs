using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Primary
{
    public class MMovement : BaseSkill
    {
        public MMovement()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.MasterySection;
        }
    }
}
