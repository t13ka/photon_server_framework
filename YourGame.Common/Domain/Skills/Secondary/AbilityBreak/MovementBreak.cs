namespace YourGame.Common.Domain.Skills.Secondary.AbilityBreak
{
    using YourGame.Common.Domain.Enums;

    public class MovementBreak : BaseSkill
    {
        public MovementBreak()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 15;
            Priority = 150;
        }
    }
}