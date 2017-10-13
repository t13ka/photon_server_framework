using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Primary
{
    public class MMastery : BaseSkill
    {
        public MMastery()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.MasterySection;
        }
    }
}
