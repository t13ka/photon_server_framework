namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class DResistance : BaseSkill
    {
        public DResistance()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.DefenceSection;
        }
    }
}
