namespace YourGame.Common.Domain
{
    using System;
    using System.Collections.Generic;

    using YourGame.Common.Domain.Enums;

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

        public bool Online;

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
            RegistrationDateTime = DateTime.UtcNow;
            NoneInventoryItems = new List<IEntity>();
            LevelFeatures = new List<LvlRewardFeaturesTypes>();
            Banned = false;
            Crystals = 1000;
            Gold = 100000;

            FirstTaskList = new List<IEntity>();

            // add modules to inventory
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
            RegistrationDateTime = DateTime.UtcNow;
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
        }

        public Player()
        {
            NoneInventoryItems = new List<IEntity>();
        }

        #endregion
    }
}