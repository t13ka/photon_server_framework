using Warsmiths.Common.Domain.CommonCharacterProfile.Attack;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Groups
{
    public class OffenceGroup
    {
        public int Summary;

        public MeleeAttackPrimary MeleeAttack;
        public RangedAttackPrimary RangedAttack;
        public MagicAttackPrimary MagicAttack;

        public OffenceGroup()
        {
            MeleeAttack = new MeleeAttackPrimary();
            RangedAttack = new RangedAttackPrimary();
            MagicAttack = new MagicAttackPrimary();

            Summary = 0;
        }

        public void Calculate(Character c)
        {
            RangedAttack.Calculate(c);
            MagicAttack.Calculate(c);
            MeleeAttack.Calculate(c);
        }
    }
}
