using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warsmiths.Common.Domain;
using Warsmiths.Server.Factories;

namespace Common.Tests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var p = PlayerFactory.CreateDefaultPlayerAccount("test", "test", "test", "test"," test" , new List<IEntity>());

            // pack
           // p.TryPackToInventory(new RedBlueCrystal { Quantity = 1 });
           // p.TryPackToInventory(new RedBlueCrystal { Quantity = 3 });
            //p.TryPackToInventory(new RedBlueCrystal { Quantity = 5 });

            // unpack
            //p.TryUnpackFromInventory(new RedBlueCrystal { Quantity = 5 });
            //p.TryUnpackFromInventory(new RedBlueCrystal { Quantity = 3 });
            //p.TryUnpackFromInventory(new RedBlueCrystal { Quantity = 1 });
        }
    }
}
