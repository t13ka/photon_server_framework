using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Skills.Secondary.Others
{
    public class ModulesEfficiency : BaseSkill
    {
        public ModulesEfficiency()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.MasterySection;
            BpPercent = 25;
            Priority = 200;
        }
    }
}
