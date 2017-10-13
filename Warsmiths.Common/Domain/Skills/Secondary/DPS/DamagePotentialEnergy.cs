using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.DPS
{
    public class DamagePotentialEnergy : BaseSkill
    {
        public DamagePotentialEnergy()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 6;
            Priority = 150;
        }
    }
}