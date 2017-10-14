namespace YourGame.Common.Domain.Skills.Secondary.Others
{
    using YourGame.Common.Domain.Enums;

    public class Weakness : BaseSkill
    {
        public Weakness()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.MasterySection;
        }
    }
}