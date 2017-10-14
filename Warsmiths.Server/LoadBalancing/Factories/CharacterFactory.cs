using System.Collections.Generic;

using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Factories
{
    using YourGame.Common.Domain;
    using YourGame.Common.Domain.CommonCharacterProfile;
    using YourGame.Common.Domain.Enums;
    using YourGame.Common.Domain.Equipment;

    public class CharacterFactory
    {
        public static Character CreateDefaultCharacter()
        {
            return new Character();
        }

        public static Character CreateDefaultCharacter(
            string name,
            ClassTypes classType,
            HeroTypes heroType,
            RaceTypes raceType)
        {
            var character = new Character
                                {
                                    Name = name,
                                    RaceType = raceType,
                                    HeroType = heroType,
                                    CommonProfile = new CommonCharacterProfile(),
                                    TasksList = new List<IEntity>()
                                };
            character.CommonProfile.Init(character);
            character.Classes.Add(classType);

            var armor = EquipmentFactory.CreateArmor(ArmorTypes.Medium, character.RaceType);
            var goliath = MasterApplication.DomainConfiguration.Objects.Find(x => x.Name == "Goliath") as BaseWeapon;
            if (goliath != null)
            {
                var sword = (BaseWeapon)goliath.Clone();
                sword.RaceType = character.RaceType;
                character.Equipment.RightWeapon = sword;
            }

            // add first quests
            character.Equipment.Armor = armor;

            character.CommonProfile.Calculate();
            return character;
        }
    }
}