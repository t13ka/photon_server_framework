namespace YourGame.Common.Domain.Skills.Secondary.Others
{
    using YourGame.Common.Domain.Enums;

    public class Capabilities : BaseSkill
    {
        public Capabilities()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.MasterySection;
            BpPercent = 25;
            Priority = 200;
        }
    }
}
