namespace YourGame.Common.Domain.Skills.Secondary.DPS
{
    using YourGame.Common.Domain.Enums;

    public class DamagePotentialMelee : BaseSkill
    {
        public DamagePotentialMelee()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.AttackSection;
            BpPercent = 25;
            Priority = 200;
        }
    }
}
