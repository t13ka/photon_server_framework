﻿using System.Collections.Generic;

namespace YourGame.Server.Factories
{
    using YourGame.Common.Domain;

    public static class PlayerFactory
    {
        public static Player CreateDefaultPlayerAccount(
            string login,
            string password,
            string firstName,
            string lastName,
            string email,
            IEnumerable<IEntity> equipments)
        {
            var result = new Player(login, password, firstName, lastName, email, equipments);

            return result;
        }

        public static Player CreateDefaultPlayerAccount(
            string login,
            string password,
            string email,
            IEnumerable<IEntity> equipments)
        {
            var result = new Player(login, password, equipments);
            return result;
        }
    }
}