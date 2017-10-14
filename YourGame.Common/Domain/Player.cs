namespace YourGame.Common.Domain
{
    using System;
    using System.Collections.Generic;

    using YourGame.Common.Domain.Enums;

    public class Player : IEntity
    {
        public short Age;

        public bool Banned;

        public int Crystals;

        public string Email;

        public string FirstName;

        public List<IEntity> FirstTaskList = new List<IEntity>();

        public GenderTypes GenderType;

        public int Gold;

        public int HealBox;

        public int Keys;

        public DateTime LastEnterDateTime;

        public DateTime LastExitDateTime;

        public string LastName;

        public string LastSessionIpAddress;

        public string Login;

        public List<IEntity> NoneInventoryItems;

        public bool Online;

        public string Password;

        public RankTypes RankInClan;

        public DateTime RegistrationDateTime;

        public Player(string login, string password, IEnumerable<IEntity> equipments)
        {
            _id = Guid.NewGuid().ToString();
            Login = login;
            Password = password;
            RegistrationDateTime = DateTime.UtcNow;
            NoneInventoryItems = new List<IEntity>();
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
    }
}