using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.Others
{
    public class CriticalHitPercent : BaseSkill
    {
        public CriticalHitPercent()
        {
            SkillGroupType = SkillGroupTypes.Secondary2;
            SkillSection = SkillSection.MasterySection;
            Set(50);
        }
    }
}
