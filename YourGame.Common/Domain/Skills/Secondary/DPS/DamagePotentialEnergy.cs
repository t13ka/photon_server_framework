namespace YourGame.Common.Domain.Skills.Secondary.DPS
{
    using YourGame.Common.Domain.Enums;

    public class DamagePotentialEnergy : BaseSkill
    {
        public DamagePotentialEnergy()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 6;
            Priority = 150;
        }
    }
}