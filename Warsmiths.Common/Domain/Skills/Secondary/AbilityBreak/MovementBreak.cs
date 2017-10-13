using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.AbilityBreak
{
    public class MovementBreak : BaseSkill
    {
        public MovementBreak()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 15;
            Priority = 150;
        }
    }
}