namespace YourGame.Common.Domain.Skills.Secondary.DPS
{
    using YourGame.Common.Domain.Enums;

    public class DamagePotentialRange : BaseSkill
    {
        public DamagePotentialRange()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 8;
            Priority = 200;
        }
    }
}
