using Warsmiths.Common.Domain.CommonCharacterProfile.Defense;


namespace Warsmiths.Common.Domain.CommonCharacterProfile.Groups
{
    public class DefenceGroup
    {
        public int Summary;

        public MeleeDefensePrimary MeleeDefense;
        public RangedDefensePrimary RangedDefense;
        public MagicDefensePrimary MagicDefense;

        public DefenceGroup()
        {
            MeleeDefense = new MeleeDefensePrimary();
            RangedDefense = new RangedDefensePrimary();
            MagicDefense = new MagicDefensePrimary();
        }

        public void Calculate(Character c)
        {
            MeleeDefense.Calculate(c);
            RangedDefense.Calculate(c);
            MagicDefense.Calculate(c);

            Summary = 0;
        }
    }
}
