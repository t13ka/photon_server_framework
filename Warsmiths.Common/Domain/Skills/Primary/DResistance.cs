using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Primary
{
    public class DResistance : BaseSkill
    {
        public DResistance()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.DefenceSection;
        }
    }
}
