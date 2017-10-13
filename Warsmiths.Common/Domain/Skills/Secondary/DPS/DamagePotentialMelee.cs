using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.DPS
{
    public class DamagePotentialMelee : BaseSkill
    {
        public DamagePotentialMelee()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 25;
            Priority = 200;
        }
    }
}
