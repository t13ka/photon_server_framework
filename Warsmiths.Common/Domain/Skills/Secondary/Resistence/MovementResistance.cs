using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.Resistence
{
    public class MovementResistance : BaseSkill
    {
        public MovementResistance()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.DefenceSection;
            BpPercent = 25;
            Priority = 100;
        }
    }
}
