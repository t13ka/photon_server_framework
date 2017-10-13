using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.Others
{
    public class CriticalHitStrength : BaseSkill
    {
        public CriticalHitStrength()
        {
            SkillGroupType = SkillGroupTypes.Secondary2;
            SkillSection = SkillSection.MasterySection;
            Set(30);
        }
    }
}
