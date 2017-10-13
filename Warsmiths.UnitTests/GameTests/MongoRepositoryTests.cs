using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Factories;
using Warsmiths.Server.Framework.DataBaseService;

namespace Common.Tests
{
    [TestClass]
    public class MongoRepositoryTests
    {
        private class MyModule : NinjectModule
        {
            public override void Load()
            {
                Bind(typeof (IRepository<Player>)).To(typeof (PlayerRepository));
            }
        }

        private class MyService
        {
            private readonly IRepository<Player> _repository;

            public MyService(IRepository<Player> repository)
            {
                _repository = repository;
            }

            public void DoTransaction()
            {
                var p = PlayerFactory.CreateDefaultPlayerAccount("2", "2","", new List<BaseEquipment>());
                _repository.Create(p);
                var results = _repository.SearchFor(t => t.Login == "2");
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            //Warsmiths.DatabaseService.BsonMappingConfigurator.Configure();
            IKernel kernel = new StandardKernel(new MyModule());
            var service = kernel.Get<MyService>();
            service.DoTransaction();
        }
    }
}