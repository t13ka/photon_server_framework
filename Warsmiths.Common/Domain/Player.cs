using System;
using System.Collections.Generic;
using System.Linq;

using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;

namespace Warsmiths.Common.Domain
{
    public class Player : IEntity
    {
        #region Props

        public string Login;

        public string Password;

        public string Email;

        public string FirstName;

        public string LastName;

        public DateTime RegistrationDateTime;

        public DateTime LastEnterDateTime;

        public DateTime LastExitDateTime;

        public string LastSessionIpAddress;

        public GenderTypes GenderType;

        public int Gold;

        public int Crystals;

        public int Keys;

        public int HealBox;

        public bool Banned;

        public bool Donater;

        public short Age;

        public RankTypes RankInClan;

        public League CurrentLeague;

        public Clan CurrentClan;

        public bool Online;

        public List<Character> Characters;

        public PlayerInventory Inventory;

        public List<IEntity> NoneInventoryItems;

        public List<IEntity> FirstTaskList = new List<IEntity>();

        public List<LvlRewardFeaturesTypes> LevelFeatures;

        #endregion

        #region Ctor

        public Player(string login, string password, IEnumerable<IEntity> equipments)
        {
            _id = Guid.NewGuid().ToString();
            Login = login;
            Password = password;
            Inventory = new PlayerInventory();
            RegistrationDateTime = DateTime.UtcNow;
            Characters = new List<Character>();
            NoneInventoryItems = new List<IEntity>();
            LevelFeatures = new List<LvlRewardFeaturesTypes>();
            Banned = false;
            Crystals = 1000;
            Gold = 100000;

            FirstTaskList = new List<IEntity>();
            foreach (var baseEquipment in equipments)
            {
                TryAddToInventory(baseEquipment);
            }

            GetFirstItems();

            // add modules to inventory
        }

        public void GetFirstItems()
        {
            NoneInventoryItems.Add(new BasePerk { Name = "Add", Number = "A1" });
            NoneInventoryItems.Add(new BasePerk { Name = "Chip", Number = "D1" });
            NoneInventoryItems.Add(new BasePerk { Name = "Move", Number = "B1" });
            NoneInventoryItems.Add(new BasePerk { Name = "BoostsSlots", Number = "J1" });
        }

        public Player(
            string login,
            string password,
            string firstname,
            string lastname,
            string email,
            IEnumerable<IEntity> equipments)
        {
            _id = Guid.NewGuid().ToString();
            Login = login;
            Password = password;
            Inventory = new PlayerInventory();
            RegistrationDateTime = DateTime.UtcNow;
            Characters = new List<Character>();
            NoneInventoryItems = new List<IEntity>();
            LevelFeatures = new List<LvlRewardFeaturesTypes>();
            Banned = false;
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            Crystals = 0;
            Gold = 1000;
            Keys = 0;
            FirstTaskList = new List<IEntity>();
            foreach (var baseEquipment in equipments)
            {
                TryAddToInventory(baseEquipment);
            }

            GetFirstItems();

            // add modules to inventory
        }

        public Player()
        {
            Inventory = new PlayerInventory();
            Characters = new List<Character>();
            NoneInventoryItems = new List<IEntity>();
        }

        #endregion

        #region Methods

        public bool TryAddToInventory(IEntity entity)
        {
            return Inventory.Add(new InventoryItem(entity, this));
        }

        public bool TryRemoveFromInventory(IEntity entity)
        {
            return Inventory.Remove(entity._id);
        }

        public bool TryRemoveFromInventory(string entityId)
        {
            return Inventory.Remove(entityId);
        }

        public bool TryPackToInventory(IEntity entity)
        {
            return Inventory.TryPack(new InventoryItem(entity, this));
        }

        public bool TryUnpackFromInventory(IEntity entity)
        {
            return Inventory.TryUnpack(new InventoryItem(entity, this));
        }

        public bool TryGetInventoryValue(string entityId, out IEntity entity)
        {
            return Inventory.TryGetValue(entityId, out entity);
        }

        /// <summary>
        /// Ищет еквипмент как в инвентаре игрока, так и надетый на его чарактеров
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="searchInCharactersWearingEquipments">Искать у чарактеров</param>
        /// <returns></returns>
        public T TryGetEquipmentFromWholePlayer<T>(string entityId, bool searchInCharactersWearingEquipments = true)
        {
            IEntity entity;
            var present = Inventory.TryGetValue(entityId, out entity);
            if (present)
            {
                var obj = (object)entity;
                return (T)obj;
            }

            if (searchInCharactersWearingEquipments)
            {
                if (Characters != null)
                {
                    // искать у чарактеров
                    foreach (var currentPlayerCharacter in Characters)
                    {
                        return TryGetEquipmentFromCharacterEquipments<T>(currentPlayerCharacter, entityId);
                    }
                }
            }

            return default(T);
        }

        public bool IsPlayerHasEquipment(string id, bool searchInCharactersWearingEquipments = true)
        {
            IEntity entity;
            var result = Inventory.TryGetValue(id, out entity);

            if (searchInCharactersWearingEquipments)
            {
                if (Characters != null)
                {
                    // искать у чарактеров
                    foreach (var currentPlayerCharacter in Characters)
                    {
                        var characterEquipment =
                            TryGetEquipmentFromCharacterEquipments<object>(currentPlayerCharacter, id);
                        if (characterEquipment != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return result;
        }

        public T TryGetEquipmentFromCharacterEquipments<T>(Character character, string equipmentId)
        {
            BaseEquipment characterEquipment = null;

            if (character.Equipment.Armor != null && character.Equipment.Armor._id == equipmentId)
                characterEquipment = character.Equipment.Armor;
            else if (character.Equipment.RightWeapon != null && character.Equipment.RightWeapon._id == equipmentId)
                characterEquipment = character.Equipment.RightWeapon;
            else if (character.Equipment.LeftWeapon != null && character.Equipment.LeftWeapon._id == equipmentId)
                characterEquipment = character.Equipment.LeftWeapon;
            if (characterEquipment == null) return default(T);
            return (T)(object)characterEquipment;
        }

        public Character GetCurrentCharacter()
        {
            Character character = null;
            if (Characters != null)
            {
                character = Characters.FirstOrDefault(t => t.Selected);
            }

            return character;
        }

        public Character GetCharacterByName(string characterName)
        {
            Character character = null;
            if (Characters != null)
            {
                character = Characters.FirstOrDefault(t => t.Name == characterName);
            }

            return character;
        }

        public void RemoveCharacter(Character c)
        {
            Characters.Remove(c);
        }

        public void ChangeSelectedCharacterTo(string characterName)
        {
            foreach (var character in Characters)
            {
                character.Selected = false;
            }

            var newSelectedCharacter = Characters.FirstOrDefault(t => t.Name == characterName);
            if (newSelectedCharacter != null)
            {
                newSelectedCharacter.Selected = true;
            }
            else
            {
                throw new Exception("Select new character failed");
            }
        }

        #endregion
    }
}