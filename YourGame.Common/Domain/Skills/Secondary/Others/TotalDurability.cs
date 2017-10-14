namespace YourGame.Common.Domain.Skills.Secondary.Others
{
    using YourGame.Common.Domain.Enums;

    public class TotalDurability : BaseSkill
    {
        public TotalDurability()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.DefenceSection;
            BpPercent = 10;
            Priority = 300;
        }
    }
}
