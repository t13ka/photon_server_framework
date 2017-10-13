using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.AbilityBreak
{
    public class AbilityBreak : BaseSkill
    {
        public AbilityBreak()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 50;
            Priority = 150;
        }
    }
}
