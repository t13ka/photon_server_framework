namespace YourGame.Common.Domain.Skills.Secondary.Resistence
{
    using YourGame.Common.Domain.Enums;

    public class MovementResistance : BaseSkill
    {
        public MovementResistance()
        {
            SkillGroupType = SkillGroupTypes.Secondary1;
            SkillSection = SkillSection.DefenceSection;
            BpPercent = 25;
            Priority = 100;
        }
    }
}
