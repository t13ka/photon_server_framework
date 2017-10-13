using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Results;
using Warsmiths.Server.NinjectConfigModules;
using Warsmiths.Server.Services.Economic;

namespace Common.Tests
{
    [TestClass]
    public class EconomicTests
    {
        private readonly EconomicRuntimeService _economicService;
        private readonly DomainConfiguration _domainConfiguration;

        public EconomicTests()
        {
            IKernel kernel = new StandardKernel(new EconomicInjectionModule(true));
            _economicService = kernel.Get<EconomicRuntimeService>();
        }

        [TestMethod]
        public void EconomicTestBuySellToServer()
        {
           /* var equipments = _domainConfiguration.GetAll();
            var p = PlayerFactory.CreateDefaultPlayerAccount("tester", "tester", "", equipments);
            p.Gold = 999999999999;
            var element = _domainConfiguration.Get<RedBlueCrystal>();
            element.Quantity = 10000;

            var order = new OrderElementStatement(p._id, element._id, 1, true);
            p.TryAddToInventory(element);

            Debug.WriteLine(" ================ Purchase ================ ");

            for (var i = 0; i < 1000; i++)
            {
                _economicService.TransactionProcessing(order);
            }

            Debug.WriteLine("================ Sale ================");
            var sale = new SaleElementStatement(p._id, element._id, 1);
            for (var i = 0; i < 1000; i++)
            {
                _economicService.TransactionProcessing(sale);
            }

            Debug.WriteLine("================ Order Sale ================");
            var rnd = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var order2 = new OrderElementStatement(p._id, element._id, rnd.Next(1, 20), true);
                var sale2 = new SaleElementStatement(p._id, element._id, rnd.Next(1, 20));
                _economicService.TransactionProcessing(order2);
                _economicService.TransactionProcessing(sale2);
            }*/
        }

        [TestMethod]
        public void EconomicTestDestroyItem()
        {
            var results = new List<DestroyEquipmentResult>();
            /*var item1 = _domainConfiguration.Get<SonicDagger>();

            for (var i = 0; i < 10000; i++)
            {
                var result = _economicService.DestroyEquipment(item1);
                results.Add(result);
            }
            */
            var results2 = results.GroupBy(t => t.Element._id).OrderByDescending(t => t.Count());
        }
    }
}