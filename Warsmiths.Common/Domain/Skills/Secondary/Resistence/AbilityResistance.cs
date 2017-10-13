using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.Resistence
{
    public class AbilityResistance : BaseSkill
    {
        public AbilityResistance()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.DefenceSection;
            BpPercent = 75;
            Priority = 100;
        }
    }
}