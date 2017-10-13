namespace Warsmiths.Common.Domain.Characteristics
{
    public class CharacteristicsBase : IEntity
    {
        public CharacteristicItem Speed ;
        public CharacteristicItem Power ;
        public CharacteristicItem Intellect ;
        public CharacteristicItem Wisdom ;
        public CharacteristicItem Endurance ;
        public CharacteristicItem Dexterity ;
        public CharacteristicItem Evasion ;

        public CharacteristicsBase()
        {
            Speed = new CharacteristicItem();
            Power = new CharacteristicItem();
            Intellect =  new CharacteristicItem();
            Endurance =  new CharacteristicItem();
            Wisdom = new CharacteristicItem();
            Dexterity = new CharacteristicItem();
        }
    }
}