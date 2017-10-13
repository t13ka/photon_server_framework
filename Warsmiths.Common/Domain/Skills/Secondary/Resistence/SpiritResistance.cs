using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.Resistence
{
    public class SpiritResistance : BaseSkill
    {
        public SpiritResistance()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.DefenceSection;
            BpPercent = 12;
            Priority = 100;
        }
    }
}
