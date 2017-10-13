using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.AbilityBreak
{
    public class SpiritBreak : BaseSkill
    {
        public SpiritBreak()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 8;
            Priority = 150;
        }
    }
}