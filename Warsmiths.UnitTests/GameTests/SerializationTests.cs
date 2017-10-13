using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Common.Tests
{
    [TestClass]
    public class SerializationTests
    {
        /*private readonly EconomicRuntimeService _service;
        private readonly DomainConfiguration _domainConfiguration;

        public SerializationTests()
        {
            //BsonMappingConfigurator.Configure();
            IKernel kernel = new StandardKernel(new EconomicInjectionModule());
            //_service = kernel.Get<EconomicRuntimeService>();
            _domainConfiguration = new DomainConfiguration(true);
        }

        [TestMethod]
        public void TestMethod_DeSerializeDestroyResults()
        {
            var item = _domainConfiguration.Get<DiscSword>();
            var destroyResult = _service.DestroyEquipment(item);
            var serializedResult = destroyResult.ToBson();
            var deserialized = BsonSerializer.Deserialize<DestroyEquipmentResult>(serializedResult);
            Assert.AreNotEqual(deserialized.Element.Quantity, 0);
        }

        [TestMethod]
        public void TestMethod_DeSerializeMorphedConvertions()
        {
            var item = _domainConfiguration.Get<MiddleArmor>();
            var serializedResult = item.ToBson();
            var deserialized = BsonSerializer.Deserialize<BaseEquipment>(serializedResult);
            Assert.AreNotEqual(deserialized, null);
        }

        [TestMethod]
        public void TestMethod_DeSerializeProfile()
        {
            var player = PlayerFactory.CreateDefaultPlayerAccount("test", "Test", "", new List<BaseEquipment>());
            player.Characters = new List<Character>();
            player.Inventory.Add(new InventoryItem(new SonicDagger(), player));

            var character = CharacterFactory.CreateDefaultCharacter();
            character.Equipment.Add(new Haze() {Place = EquipmentPlaceTypes.Armor });
            character.Reserved1 = new List<string> { "test", "test2", "test2" };
            player.Characters.Add(character);

            var data = player.ToBson();
            var deserializedPlayer = BsonSerializer.Deserialize<Player>(data);
            var cliendDeserializedPlayer = data.FromBson<Player>();

            Assert.AreEqual(player._id, deserializedPlayer._id);
        }

        [TestMethod]
        public void TestDataBase1()
        {
            BsonMappingConfigurator.Configure();
            var testPlayer = PlayerFactory.CreateDefaultPlayerAccount("1", "1", "1", "1", "1", new List<BaseEquipment>());

            testPlayer.Characters.Add(CharacterFactory.CreateDefaultCharacter("test", ClassTypes.Chosen,
                HeroTypes.CleanKing, RaceTypes.Animit));
            testPlayer.Characters.Add(CharacterFactory.CreateDefaultCharacter("test", ClassTypes.Alchemist,
                HeroTypes.CleanKing, RaceTypes.Animit));

            var data = testPlayer.ToBson();
            var extracted = FromBson<Player>(data);
            Assert.AreEqual(testPlayer.Inventory.Count, extracted.Inventory.Count);
        }

        public static T FromBson<T>(byte[] data)
        {
            var stream = new MemoryStream(data);
            stream.Seek(0, SeekOrigin.Begin);
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
            serializer.CheckAdditionalContent = true;

            var reader = new Newtonsoft.Json.Bson.BsonReader(stream);
            serializer.Converters.Add(new WarsmithsClientConverter());
            return serializer.Deserialize<T>(reader);
        }

        [TestMethod]
        public void TestDataBase2()
        {
            //BsonMappingConfigurator.Configure();
            var eq = new List<BaseEquipment>();
            var ha = _domainConfiguration.Get<HeavyArmor>();
            ha.ArmorParts = new List<ArmorPart>
                {
                    new ArmorPart {ArmorPartType = ArmorPartTypes.Back},
                    new ArmorPart {ArmorPartType = ArmorPartTypes.Chest},
                    new ArmorPart {ArmorPartType = ArmorPartTypes.RightHand},
                    new ArmorPart {ArmorPartType = ArmorPartTypes.LeftHand},
                    new ArmorPart {ArmorPartType = ArmorPartTypes.RightLeg},
                    new ArmorPart {ArmorPartType = ArmorPartTypes.LeftLeg},
                };
            eq.Add(ha);
            var testPlayer = PlayerFactory.CreateDefaultPlayerAccount("1", "1", "1", "1", "1", eq);
            var data = testPlayer.ToBson();
            var extracted = FromBson<Player>(data);
            Assert.AreEqual(testPlayer.Inventory.Count, extracted.Inventory.Count);
        }*/

    }
}