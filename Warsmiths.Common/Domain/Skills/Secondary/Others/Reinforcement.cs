using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.Others
{
    public class Reinforcement : BaseSkill
    {
        public Reinforcement()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.MasterySection;
        }
    }
}