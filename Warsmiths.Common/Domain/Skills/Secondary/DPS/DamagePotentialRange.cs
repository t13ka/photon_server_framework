using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.DPS
{
    public class DamagePotentialRange : BaseSkill
    {
        public DamagePotentialRange()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 8;
            Priority = 200;
        }
    }
}
