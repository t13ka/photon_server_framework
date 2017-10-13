using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums.ItemGeneration;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Server.Factories;

namespace Common.Tests
{
    [TestClass]
    public class WeaponsTests
    {
        private readonly DomainConfiguration _domainConfiguration;
        public WeaponsTests()
        {
            _domainConfiguration = new DomainConfiguration(true);
        }

        [TestMethod]
        public void TestSharpening()
        {
            /*var weapon = _domainConfiguration.Get<ChainSword>();
            var p = PlayerFactory.CreateDefaultPlayerAccount("test", "test", "", new List<BaseEquipment>());
            var character = CharacterFactory.CreateDefaultCharacter();
            weapon.AssignOwner(p);
            weapon.Price = 1000;

            Assert.AreEqual(100, weapon.GetSharpeningPrice(1));
            Assert.AreEqual(105, weapon.GetSharpeningPrice(2));
            Assert.AreEqual(110, weapon.GetSharpeningPrice(3));
            Assert.AreEqual(116, weapon.GetSharpeningPrice(4));

            weapon.IncreaseSharpening(1);
            var totalWeaponPrice = weapon.GetTotalPrice();
            var damage = weapon.GetMeleeDamage(character);
            var anomality = weapon.GetTotalAnomality(character);


            Assert.AreEqual(1100, totalWeaponPrice);

            weapon.IncreaseSharpening(1);
            totalWeaponPrice = weapon.GetTotalPrice();
            damage = weapon.GetMeleeDamage(character);
            anomality = weapon.GetTotalAnomality(character);
            Assert.AreEqual(1205, totalWeaponPrice);

            weapon.IncreaseSharpening(5);

            var sh = (float)weapon.Sharpening/2;
            var r = Math.Round((decimal) (sh), MidpointRounding.ToEven);*/
        }

        [TestMethod]
        public void TestSharpeningWithDestroyChange()
        {
            /*
            var weapon = _domainConfiguration.Get<ChainSword>();
            var enchantedResults = 0;
            var crashedResults = 0;
            var character = CharacterFactory.CreateDefaultCharacter();

            for (int cl = 0; cl < 50; cl++)
            {
                Debug.WriteLine("================== CRAFT LEVEL:{0} ==================", cl);

                for (var i = 0; i < 50; i++)
                {
                    //var r = weapon.TryEnchant(45, 1);
                    //if (r.Success)
                    //{
                    //    enchantedResults++;
                    //}
                    //else
                    //{
                    //    crashedResults++;
                    //}
                    var test = weapon.CalcucaleAndGetEnchantInfo(character, new BlueCrystal(), i);

                    Debug.WriteLine(test);
                }
            }

            Assert.AreNotEqual(enchantedResults, crashedResults);
            Assert.AreEqual(true, weapon.TryEnchant(1, 1));*/
        }

        /*[TestMethod]
        /*public void TestEquipmentGenerator()
        {
            var l = new List<BaseEquipment>();
            var domainConfig = new DomainConfiguration(true);
            for (int i = 0; i < 100; i++)
            {
                var item = domainConfig.GenerateEquipment(ItemGenerationMasteryTypes.Legend,
                    ItemGenerationLuckTypes.Rare, ItemGenerationMutualAidTypes.Regular);

                if (item != null)
                {
                    l.Add(item);
                }
                else
                {
                    Debug.WriteLine("error");
                }
            }

            var g = l.GroupBy(t => t.Rarety);
        }*/

        [TestMethod]
        public void PriceAnomalityDamageChronograph()
        {
            /*var weapon = _domainConfiguration.Get<Chronograph>();
            var character = CharacterFactory.CreateDefaultCharacter();

            // damage
            var r1 = weapon.GetMeleeDamage(character);
            weapon.Sharpening++;
            var r2 = weapon.GetMeleeDamage(character);

            // price
            var p1 = weapon.GetTotalPrice();
            weapon.Sharpening++;
            var p2 = weapon.GetTotalPrice();

            // anomality
            var a1 = weapon.GetTotalAnomality(character);
            weapon.Sharpening++;
            var a2 = weapon.GetTotalAnomality(character);*/
        }

        [TestMethod]
        public void PriceAnomalityArmor()
        {
            /*var armor = _domainConfiguration.Get<HeavyArmor>();
            armor.InitializeDefaultArmorParts();
            var totalArmorAnomality = armor.GetTotalAnomality();*/
        }
        /*
        [TestMethod]
        public void PutOnOthersWeaponsTest()
        {
            var character = CharacterFactory.CreateDefaultCharacter();
            var oneHandWeapon1 = _domainConfiguration.Get<AggregatorDagger>();
            var oneHandWeapon2 = _domainConfiguration.Get<SonicDagger>();
            var twoHandWeapon1 = _domainConfiguration.Get<DiscSword>();
            var twoHandWeapon2 = _domainConfiguration.Get<Chronograph>();
            var automate = _domainConfiguration.Get<Goliath>();
            var staff = _domainConfiguration.Get<Pointer>();
            var twoHandedAutomate1 = _domainConfiguration.Get<Cracker>();
            var twoHandedAutomate2 = _domainConfiguration.Get<ShotgunDvach>();

            var shield1 = _domainConfiguration.Get<VoidShield>();
            var shield2 = _domainConfiguration.Get<Trimmer>();

            var resonatorSword = _domainConfiguration.Get<ResonatorSword>();
            var sonicSword = _domainConfiguration.Get<SonicSword>();

            WearingResult result;

            // двуручное можно взять только в правую
            result = character.Equipment.PutOn(twoHandWeapon1, EquipmentPlaceTypes.RightHand);
            result.Success.ShouldBe(true);

            character.Equipment.Clear();

            // двуручное нельзя взять в левую
            result = character.Equipment.PutOn(twoHandWeapon1, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(false);

            character.Equipment.Clear();

            // 2 двуручных в разные руки так же нельзя взять
            result = character.Equipment.PutOn(twoHandWeapon1, EquipmentPlaceTypes.RightHand);
            result = character.Equipment.PutOn(twoHandWeapon2, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(false);

            character.Equipment.Clear();

            // берем двуручное в правую руку, потом пробуем взять в эту же руку другое двуручное. ожидаем что первое снимется
            result = character.Equipment.PutOn(twoHandWeapon1, EquipmentPlaceTypes.RightHand);
            result = character.Equipment.PutOn(twoHandWeapon2, EquipmentPlaceTypes.RightHand);
            result.Success.ShouldBe(true);

            character.Equipment.Clear();

            // нельзя взять двуручное и одноручное
            result = character.Equipment.PutOn(twoHandWeapon1, EquipmentPlaceTypes.RightHand);
            result = character.Equipment.PutOn(oneHandWeapon1, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(false);

            character.Equipment.Clear();

            // нельзя взять автомат и посох в разные руки
            result = character.Equipment.PutOn(automate, EquipmentPlaceTypes.RightHand);
            result = character.Equipment.PutOn(staff, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(false);

            character.Equipment.Clear();

            // нельзя взять двуручный автомат в правую, и двуручный автомат в левую
            result = character.Equipment.PutOn(twoHandedAutomate1, EquipmentPlaceTypes.RightHand);
            result = character.Equipment.PutOn(twoHandedAutomate2, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(false);

            character.Equipment.Clear();

            // можно вставить 2 одноручных в разные руки. 
            result = character.Equipment.PutOn(resonatorSword, EquipmentPlaceTypes.RightHand);
            result = character.Equipment.PutOn(sonicSword, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(true);

            // проверяем замену оружия в правой руке
            result = character.Equipment.PutOn(oneHandWeapon1, EquipmentPlaceTypes.RightHand);
            result.TakeOffEquipments.Count.ShouldBe(1);

            // проверяем замену оружия в левой руке
            result = character.Equipment.PutOn(oneHandWeapon2, EquipmentPlaceTypes.LeftHand);
            result.TakeOffEquipments.Count.ShouldBe(1);

            character.Equipment.Clear();

            // проверяем чтобы нельзя было одеть 2 щита в 2 руки
            result = character.Equipment.PutOn(shield1, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(true);
            result = character.Equipment.PutOn(shield1, EquipmentPlaceTypes.RightHand);
            result.Success.ShouldBe(false);

            character.Equipment.Clear();

            // пробуем одеть одноручный меч и щит
            result = character.Equipment.PutOn(oneHandWeapon1, EquipmentPlaceTypes.RightHand);
            result.Success.ShouldBe(true);
            result = character.Equipment.PutOn(shield1, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(true);

            character.Equipment.Clear();

            // пробуем одеть двуручны меч и щит
            result = character.Equipment.PutOn(twoHandWeapon1, EquipmentPlaceTypes.RightHand);
            result.Success.ShouldBe(true);
            result = character.Equipment.PutOn(shield1, EquipmentPlaceTypes.LeftHand);
            result.Success.ShouldBe(false);
        }*/
    }
}