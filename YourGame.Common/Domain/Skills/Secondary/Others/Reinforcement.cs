namespace YourGame.Common.Domain.Skills.Secondary.Others
{
    using YourGame.Common.Domain.Enums;

    public class Reinforcement : BaseSkill
    {
        public Reinforcement()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.MasterySection;
        }
    }
}