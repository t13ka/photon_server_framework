using System;
using System.Linq;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Factories;
using Warsmiths.Common.Utils;
using Warsmiths.DatabaseService.Repositories;

namespace Warsmiths.DatabaseTestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Testing();

            // randomize item
            //TestRandomizeMorph();

            // STATS
            //StatsTesting();

            // destroy item
            //ItemDestroy();

            // AUCTION
            //var c = CharacterFactory.CreateDefaultCharacter();
            //Console.WriteLine(c.ToString());
            //var sword = new DiscSword();
            //if (!c.Equipment.TryWear(sword, EquipmentPlaceTypes.LeftHand))
            //{
            //    Console.WriteLine("Fuuuuuuuuuuuuuuuck");
            //}
            //Console.ReadKey();
            //var l = c.SkillSet.ToList();

            //c.Update();
            //Console.WriteLine(c.ToString());

            //Console.ReadKey();
            //return;

            //SubscribeAuctionEvents();

            //AuctionTesting();
            //AuctionTesting2();
        }

        public static void Testing()
        {
            var ss = new PlayerRepository();
            var testPlayer = PlayerFactory.CreateDefaultPlayerAccount(Guid.NewGuid().ToString(), "123", "tim", "timq",
                "qweqe");
            ss.Create(testPlayer);

            var resu = ss.GetById(testPlayer._id);
        }

        private static void StatsTesting()
        {
            var step = 1;
            var c = CharacterFactory.CreateDefaultCharacter();
            c.Name = "Test wizard";
            Console.WriteLine("--- both right hand ---");

            var funcInc = new Action<float>(characteristic => { characteristic++; });
            var funcDec = new Action<float>(characteristic => { characteristic--; });
            for (var i = 1; i < 15; i++)
            {
                c.Level = i;
                foreach (var item1 in EquipmentUtils.EquipmentsDictionary)
                {
                    foreach (var item2 in EquipmentUtils.EquipmentsDictionary.OrderBy(t => t.Key))
                    {
                        step = ChangeEquipmentAndPrint(c, item1.Value, item2.Value, step);

                        ConsoleKey pressed;
                        do
                        {
                            pressed = Console.ReadKey().Key;
                            switch (pressed)
                            {
                                case ConsoleKey.Delete:
                                    return;

                                case ConsoleKey.Z:
                                    break;

                                case ConsoleKey.Q:
                                    funcInc(c.Characteristics.Body);
                                    break;
                                case ConsoleKey.A:
                                    funcDec(c.Characteristics.Body);
                                    break;
                                case ConsoleKey.W:
                                    funcInc(c.Characteristics.Dexterity);
                                    break;
                                case ConsoleKey.S:
                                    funcDec(c.Characteristics.Dexterity);
                                    break;
                                case ConsoleKey.E:
                                    funcInc(c.Characteristics.Evasion);
                                    break;
                                case ConsoleKey.D:
                                    funcDec(c.Characteristics.Evasion);
                                    break;
                                case ConsoleKey.R:
                                    funcInc(c.Characteristics.Intelligence);
                                    break;
                                case ConsoleKey.F:
                                    funcDec(c.Characteristics.Intelligence);
                                    break;
                                case ConsoleKey.T:
                                    funcInc(c.Characteristics.Power);
                                    break;
                                case ConsoleKey.G:
                                    funcDec(c.Characteristics.Power);
                                    break;
                                case ConsoleKey.Y:
                                    funcInc(c.Characteristics.Speed);
                                    break;
                                case ConsoleKey.H:
                                    funcDec(c.Characteristics.Speed);
                                    break;
                                case ConsoleKey.U:
                                    funcInc(c.Characteristics.Wisdom);
                                    break;
                                case ConsoleKey.J:
                                    funcDec(c.Characteristics.Wisdom);
                                    break;
                                default:
                                    continue;
                            }

                            ChangeEquipmentAndPrint(c, item1.Value, item2.Value, step);
                        } while (pressed != ConsoleKey.Z);
                    }
                }
            }

            Console.ReadKey();
        }

        private static int ChangeEquipmentAndPrint(Character c, BaseEquipment item1,
            BaseEquipment item2, int step)
        {
            Console.Clear();

            c.Equipment.TryWear(item1, EquipmentPlaceTypes.LeftHand);
            c.Equipment.TryWear(item2, EquipmentPlaceTypes.RightHand);
            c.Update();
            Console.WriteLine(c.ToString(CharacterToStringType.All));
            step++;

            c.Equipment.TryUnwear(EquipmentPlaceTypes.LeftHand);
            c.Equipment.TryUnwear(EquipmentPlaceTypes.RightHand);
            Console.WriteLine("--- press 'z' to equip new item --- to step:" + step);
            Console.WriteLine("--- press 'delete' key to EXIT ---");
            Console.WriteLine("--- press 'space' key to show next level ---");
            Console.WriteLine("--- press 'q' key to increase Body, 'a' - to decriase ---");
            Console.WriteLine("--- press 'w' key to increase Dexterity, 's' - to decriase ---");
            Console.WriteLine("--- press 'e' key to increase Evasion, 'd' - to decriase ---");
            Console.WriteLine("--- press 'r' key to increase Intelligence, 'f' - to decriase ---");
            Console.WriteLine("--- press 't' key to increase Power, 'g' - to decriase ---");
            Console.WriteLine("--- press 'y' key to increase Speed, 'h' - to decriase ---");
            Console.WriteLine("--- press 'u' key to increase Wisdom, 'j' - to decriase ---");

            return step;
        }


        //private static void AuctionTesting2()
        //{
        //    var presentedLot = AuctionRuntimeService.GetLotByEquipmentId("F6730FE3-4850-4216-B3B4-4397F3EA0DC4");
        //    presentedLot?.TryChangePrice(1300);
        //    Console.ReadKey();
        //}
      
        private static void TestRandomizeMorph()
        {
            var random = new Random();

            for (var i = 0; i < 100; i++)
            {
                var item = BaseWeapon.RandomGenerate(random);
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }
}