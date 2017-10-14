namespace YourGame.Common.Domain.Skills.Primary
{
    using YourGame.Common.Domain.Enums;

    public class DDefense : BaseSkill
    {
        public DDefense()
        {
            SkillGroupType = SkillGroupTypes.Primary;
            SkillSection = SkillSection.DefenceSection;
        }
    }
}
