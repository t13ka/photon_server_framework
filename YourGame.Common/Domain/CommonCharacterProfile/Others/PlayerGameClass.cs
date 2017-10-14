namespace YourGame.Common.Domain.CommonCharacterProfile.Others
{
    using YourGame.Common.Domain.Enums;

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
