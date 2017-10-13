using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;

using Warsmiths.Server.Factories;

namespace Common.Tests
{
    [TestClass]
    public class CharacterTests
    {
        private readonly DomainConfiguration _domainConfiguration = new DomainConfiguration(true);

        [TestMethod]
        public void CharacterPutOnTakeOffTestMethod1()
        {
            /*var c = CharacterFactory.CreateDefaultCharacter();

            // weapons
            var weapon1 = _domainConfiguration.Get<Cracker>();
            var weapon2 = _domainConfiguration.Get<ZeroInverter>();

            var result1 = c.Equipment.PutOn(weapon1, EquipmentPlaceTypes.RightHand);
            var result2 = c.Equipment.PutOn(weapon2, EquipmentPlaceTypes.LeftHand);
            var result3 = c.Equipment.PutOn(weapon2, EquipmentPlaceTypes.Armor);

            //armors
            /*var armor = _domainConfiguration.Get<MiddleArmor>();
            var result4 = c.Equipment.PutOn(armor, EquipmentPlaceTypes.RightHand);

            //armors in none
            var armorForNone = _domainConfiguration.Get<MiddleArmor>();
            var result5 = c.Equipment.PutOn(armorForNone, EquipmentPlaceTypes.None);*/
        }

        [TestMethod]
        public void TestCharacterLevels()
        {
            var character = CharacterFactory.CreateDefaultCharacter("", ClassTypes.Alchemist, HeroTypes.Aila,
                RaceTypes.Admar);
            // l1
            character.Experience = 0;
            character.Update();
            var l1 = character.Level;

            // l2
            character.Experience = 1100;
            character.Update();
            var l2 = character.Level;

            // l3
            character.Experience = 1300;
            character.Update();
            var l3 = character.Level;

            // l10
            character.Experience = 7452;
            character.Update();
            var l4 = character.Level;
        }

        [TestMethod]
        public void TestCharacterCommonProfile()
        {
            // DONE!
            // todo; 1
            // TODO: - т.к. раздел melee то этот раздел должен учитывать урон только от melee оружия
            // todo: -  в левую руку взяли оружие меч, то урон должен записываться в  Value 1
            // todo: если взяли 2 меча, то записываем в v1, v2 , + выставялем флаг UseSecondHand
            // todo: и это касается всех разделов группы атака (для всех оружий)


            // DONE!
            // todo: 2
            // todo: если 2 однотипных оружия, то берем урон только от того котой в правой.
            // todo: смотрим в таблицу. если видим М, то берем при равных пушках - меньшее, если Б - большее значение вторички, есле П - то оружие из правой

            // DONE!
            // todo 3;
            // todo: если одета броня то, то берем все модули по вторичке которые могут изменить вторичку (флаг влияния).
            // todo: суммируем их показатели влияния(значения) и умножаем это на текущий соотвествующий показатель вторички, и записываем в value1
            // todo: в BpPercent записывается суммарное значение процента соответвующего модуля соотвествующей вторички

            // DONE!
            // todo: 4
            // todo: specal attack
            // todo: поправить special attack в тип string из float

            // DONE!
            // todo: 5
            // todo: для каждого из раздела дамага взять свой демедж (range oneShot, ChargeDamage, MeleDamage)


            // done!
            // todo: 6
            // todo: для acuracy в range атаке если 2 однотипных автомата, шотганов домножаем урон на 0.75 для двух рук

            // DONE!
            // todo: 7
            // todo: для раздела magic атаки, берем урон1+урон2*0.75 = и записываем в value1 (не в value2, в value2 не нужно ничего писать для этого)

            // todo: 8
            // todo: раздел defense.............. плюшка щит
            // todo: щиты бывают 3-х типов. 

            // todo: 9
            // todo: "special defense", "Rarity", "anomaly", "Weight". должны изменяться только при наличии щита...
            // todo: для mele defence, для этив вторичек Value1 заполняется значением брони которая одета, 
            // а Value2 заполняется только в том случае если щит одет "Melee" типа, и выставлется флаг UseSecondValue
            // это паша уточнит. (Если тип у щита Ranged, то те же "special defense", "Rarity", "anomaly", "Weight" заполняются в разделе Ranged.. тоесть щит закрывает соответствующую атаку соотвтетвующего типа
            // special defence, просто записать имя щита. Если щит типа ranged, То имя зита записываем в Ranged, если Magic то тоже в magin записываем имя щита
            //
            // вторая рука должна заполняться в том случае если первая шмотка melee и вторая melee, при этом ставим флажок UseSecondHand. Если во второй Range пушка, но в первой.
            // todo: 10
            // 
            /*var character = CharacterFactory.CreateDefaultCharacter("", ClassTypes.Catcher, HeroTypes.Aila,
                RaceTypes.Admar);
        
            var commonCharacterProfile = new CommonCharacterProfile(character);

            commonCharacterProfile.Calculate();*/
        }
    }
}