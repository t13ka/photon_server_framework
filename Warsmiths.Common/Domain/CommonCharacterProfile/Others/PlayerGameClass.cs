using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Others
{
    public class PlayerGameClass
    {
        public int Tier;
        public string Name;
        public string Desc;
        public ClassTypes Type;
        public CharacteristicE Characteristic;

        public PlayerClassAbility[] Abilities;
        public PlayerGameClass[] Sinergy;
        public int PositionOnLine;
    }
}
