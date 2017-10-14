using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using MongoDB.Bson;
using Photon.SocketServer;

using Warsmiths.Server.Events;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Services.Economic
{
    using YourGame.Common;
    using YourGame.Common.Domain;
    using YourGame.Common.Domain.Elements;
    using YourGame.Common.Domain.Enums;
    using YourGame.Common.Domain.Equipment;
    using YourGame.Common.ListContainer;
    using YourGame.Common.Results;
    using YourGame.Server.Framework.DataBaseService;
    using YourGame.Server.Framework.Services;

    public class EconomicRuntimeService : IRuntimeService, IDisposable
    {
        #region Ctors

        public EconomicRuntimeService(IRepository<Player> repository)
        {
            _playerRepository = repository;
            _fiber = new PoolFiber();
            _fiber.ScheduleOnInterval(Update, 0, Convert.ToInt64(RefreshTimeSpan.TotalMilliseconds));
            foreach (var item in MasterApplication.DomainConfiguration.Elements)
            {
                _globalElementStatements.Add(new GlobalElementStatement
                {
                    Element = item,
                    OrderPrice = item.BasePrice,
                });
            }

            _fiber.Start();
        }

        #endregion

        #region Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static IRepository<Player> _playerRepository;

        private readonly Random _random = new Random();

        private int _currentSecondToReset;

        /// <summary>
        /// </summary>
        private readonly HashSet<PeerBase> _subscribers = new HashSet<PeerBase>();

        /// <summary>
        ///     thread fiber
        /// </summary>
        private readonly PoolFiber _fiber;

        /// <summary>
        /// </summary>
        public static TimeSpan RefreshTimeSpan => new TimeSpan(0, 0, 0, 1, 0);

        /// <summary>
        ///     ведомость для каждого элемента, сколько куплено и продано
        /// </summary>
        private readonly List<GlobalElementStatement> _globalElementStatements = new List<GlobalElementStatement>();

        private readonly List<ElementStatement> _elementStatements = new List<ElementStatement>();
        private static TimeSpan ResetTimeSpan => new TimeSpan(0, 12, 0, 0, 0);

        #endregion

        #region Props

        public IEnumerable<ElementStatement> ElementStatements => _elementStatements;

        public IEnumerable<GlobalElementStatement> GlobalElementStatements => _globalElementStatements;

        public IEnumerable<PeerBase> Subscribers => _subscribers;

        #endregion

        #region Methods

        public void Dispose()
        {
            _fiber.Stop();
            _fiber.Dispose();
        }

        private static void Reset()
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("====== DO RESET ECONOMIC SERVICE ======");
            }
        }

        public TransactionProcessingResult TransactionProcessing(ElementStatement es)
        {
            var globalElementStatement = Get(es.ElementId);

            var result = es.Handle(this, globalElementStatement);

            if (result.Success && es.Instantly == false)
            {
                lock (((ICollection) _elementStatements).SyncRoot)
                {
                    _elementStatements.Add(es);
                }
            }

            RecalculateGlobalElementStatement(es, globalElementStatement);

            return result;
        }

        public DestroyEquipmentResult DestroyEquipment(BaseEquipment equipment)
        {
            BaseElement droppedItem = null;
            var attemptCount = 1;
            var exit = false;
            var allElements = MasterApplication.DomainConfiguration.Elements;

            var jc = GetJuniorCategoryByRarety(equipment.Rarety);
            var sc = GetSeniorCategoryByRarety(equipment.Rarety);
            var elements = GetPotentialElementsByEquipmentRarety(equipment.Rarety, allElements, jc, sc);

            var dropChanceList = GetSortedByDropChanceList(elements, jc, sc);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat($"Try destroy equiment: {equipment._id}");
            }

            do
            {
                foreach (var item in dropChanceList)
                {
                    var randomNumber = _random.Next(0, 100);

                    var truncatedDropChange = Convert.ToInt32(item.DropChance);
                    if (randomNumber <= truncatedDropChange)
                    {
                        droppedItem = (BaseElement) item.Element;
                        exit = true;
                    }
                    else
                    {
                        attemptCount++;
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat(
                                $"number of attempt:{attemptCount}, random value:{randomNumber}, current item chance:{item.DropChance}");
                        }
                    }
                }
            } while (exit == false);

            droppedItem.Quantity = GetQuantityForDroppedElement(equipment, droppedItem);

            return new DestroyEquipmentResult
            {
                Element = droppedItem,
                DropElementItemResult = dropChanceList
            };
        }

        private static float DifferencePercentage(float x, float y)
        {
            if (x > y)
            {
                return (y - x)/y*100;
            }
            return (y - x)/x*100;
        }

        private void Update()
        {
            _currentSecondToReset++;

            if (_currentSecondToReset > ResetTimeSpan.TotalSeconds)
            {
                _currentSecondToReset = 0;
                Reset();
            }

            var toInform = new List<PeerBase>();

            lock (((ICollection) _elementStatements).SyncRoot)
            {
                // not completed
                foreach (var st in _elementStatements.Where(t => t.Completed == false))
                {
                    st.Update();
                }

                // completed
                foreach (var st in _elementStatements.Where(t => t.Completed))
                {
                    var recipient = _subscribers
                        .Select(t => t as MasterClientPeer)
                        .FirstOrDefault(t => t.UserId == st.PlayerId);
                    if (recipient != null)
                    {
                        toInform.Add(recipient);
                    }
                    else
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Can't send update inventory for player because user aren't online");
                        }
                    }

                    Log.DebugFormat($"Order element statement with ID={st.StatementId} has been deleted from queue!");

                    var element = MasterApplication.DomainConfiguration.Elements.Find( x => x._id == st.ElementId);

                    element.Quantity = st.Generated;

                    var player = _playerRepository.GetById(st.PlayerId);

                    player.Inventory.TryPack(new InventoryItem(element, player));

                    _playerRepository.Update(player);
                 
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug($"player: {player.Login}, earned:{st.Generated}; Element:{st.ElementId}");
                    }
                }

                // remove all completed
                _elementStatements.RemoveAll(t => t.Completed);
            }

            // inform
            foreach (var peerBase in toInform)
            {
                var peer = (MasterClientPeer) peerBase;
                peer.SendUpdatePlayerInventoryEvent();
                peer.SendUpdatePlayerProfileEvent();
                peer.SendUpdateElementsOrderEvent();
            }
        }

        private int GetQuantityForDroppedElement(BaseEquipment equipment, BaseElement droppedItem)
        {
            // цена разрушения - это акутальная цена/3
            var st = Get(droppedItem._id);
            var armor = equipment as BaseArmor;
            var weapon = equipment as BaseWeapon;
            var tp = armor?.GetTotalPrice() ?? (weapon?.GetTotalPrice() ?? ((BaseModule) equipment).GetTotalPrice());
            var truncated = Math.Truncate(tp / 3/st.BuyPrice);
            var calcQuantity = Convert.ToInt32(truncated);
            if (calcQuantity == 0)
            {
                calcQuantity = 1;
            }
            return calcQuantity;
        }

        private List<DropElementItemResult> GetSortedByDropChanceList(List<BaseElement> elements,
            ElementCategoryTypes jc,
            ElementCategoryTypes sc)
        {
            var percentageList = new List<DropElementItemResult>();
            const double jcDropChance = 70d;
            const double scDropChance = 30d;
            foreach (var e in elements)
            {
                var s = Get(e._id);
                var percent = 0d;
                var count = 0;

                if (s.Element.CategoryType == jc)
                {
                    percent = jcDropChance;
                    count = elements.Count(t => t.CategoryType == jc);
                }

                if (s.Element.CategoryType == sc)
                {
                    percent = scDropChance;
                    count = elements.Count(t => t.CategoryType == jc);
                }

                var baseChance = percent/count;

                var delta = (double) DifferencePercentage(s.Element.BasePrice, s.OrderPrice);

                var item = new DropElementItemResult
                {
                    Element = s.Element,
                    DropChance = baseChance + baseChance*delta/100
                };
                percentageList.Add(item);
            }

            var sortedList = percentageList.OrderByDescending(t => t.DropChance).ToList();
            return sortedList;
        }

        private static List<BaseElement> GetPotentialElementsByEquipmentRarety(RaretyTypes rarety,
            IEnumerable<BaseElement> allElements,
            ElementCategoryTypes jc, ElementCategoryTypes sc)
        {
            List<BaseElement> elements;
            switch (rarety)
            {
                case RaretyTypes.Regular:
                    elements = allElements.Where(t => t.CategoryType == jc || t.CategoryType == sc).ToList();
                    break;

                case RaretyTypes.Rare:
                    elements = allElements.Where(t => t.CategoryType == jc || t.CategoryType == sc).ToList();
                    break;

                case RaretyTypes.Epic:
                    elements = allElements.Where(t => t.CategoryType == jc || t.CategoryType == sc).ToList();
                    break;

                case RaretyTypes.Legend:
                    elements = allElements.Where(t => t.CategoryType == jc || t.CategoryType == sc).ToList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return elements;
        }

        private static ElementCategoryTypes GetJuniorCategoryByRarety(RaretyTypes rarety)
        {
            ElementCategoryTypes result;
            switch (rarety)
            {
                case RaretyTypes.Regular:
                    result = ElementCategoryTypes.First;
                    break;

                case RaretyTypes.Rare:
                    result = ElementCategoryTypes.Second;
                    break;

                case RaretyTypes.Epic:
                    result = ElementCategoryTypes.Third;
                    break;

                case RaretyTypes.Legend:
                    result = ElementCategoryTypes.Fourth;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

        private static ElementCategoryTypes GetSeniorCategoryByRarety(RaretyTypes rarety)
        {
            ElementCategoryTypes result;
            switch (rarety)
            {
                case RaretyTypes.Regular:
                    result = ElementCategoryTypes.Second;
                    break;

                case RaretyTypes.Rare:
                    result = ElementCategoryTypes.Third;
                    break;

                case RaretyTypes.Epic:
                    result = ElementCategoryTypes.Fourth;
                    break;

                case RaretyTypes.Legend:
                    result = ElementCategoryTypes.Fifth;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

        private void RecalculateGlobalElementStatement(ElementStatement es, GlobalElementStatement gs)
        {
            const float priceShift = 0.01f;
            long oorSold;
            long oorOrder;
            var basePrice = gs.Element.BasePrice;
            var orderPrice = gs.OrderPrice;
            var gravity = Convert.ToDouble(orderPrice/basePrice*10)/3*2;
            var online = _subscribers.Count;
            var percent = basePrice*priceShift;
            var minPrice = gs.Element.MinPrice;
            var maxPrice = gs.Element.MaxPrice;
            var maxAmount = MasterApplication.DomainConfiguration.MaxElementsAmountForPlayer;
            //double quantity = 0;
            int newPrice = 0;

            if (_subscribers.Count == 0)
            {
                online = 1;
            }

            if (orderPrice >= basePrice)
            {
                oorOrder = Convert.ToInt64(maxAmount*online + gravity);
                oorSold = Convert.ToInt64(maxAmount*online - gravity);
            }
            else
            {
                oorOrder = Convert.ToInt64(maxAmount*online - gravity);
                oorSold = Convert.ToInt64(maxAmount*online + gravity);
            }

            if (es is OrderElementStatement)
            {
                //quantity = gs.OrderedQuantity;
                if (gs.OrderedQuantity > oorOrder)
                {
                    newPrice = orderPrice + (int)percent;

                    var delta = (int) (gs.OrderedQuantity - oorOrder);
                    gs.OrderedQuantity = delta > 0 ? delta : 0;
                }
            }
            if (es is SaleElementStatement)
            {
                //quantity = gs.SoldQuantity;
                if (gs.SoldQuantity > oorSold)
                {
                    newPrice = orderPrice - (int)percent;

                    var delta = (int)(gs.SoldQuantity - oorOrder);
                    gs.SoldQuantity = delta > 0 ? delta : 0;
                }
            }

            if (newPrice > minPrice && newPrice < maxPrice)
            {
                gs.OrderPrice = newPrice;
            }

            if (gs.OrderedQuantity > oorOrder)
            {
            }
            if (gs.SoldQuantity > oorSold)
            {
                gs.SoldQuantity = 0;
            }

            /*var info =
                $"{es.GetType().Name}: element '{gs.Element._id}', qty:{quantity}, price:{gs.OrderPrice}, oor order:{oorOrder}, oor buy: {oorSold}";

            if (string.IsNullOrEmpty(info) == false)
            {
                Debug.WriteLine(info);
            }

            if (Log.IsDebugEnabled && string.IsNullOrEmpty(info) == false)
            {
                Log.DebugFormat(info);
            }*/

            if (_subscribers.Count > 0)
            {
                SendUpdateElementsPricesToSubscribers();
            }
        }

        private void SendUpdateElementsPricesToSubscribers()
        {
            var container = new ElementsPricesContainer
            {
                ElementPriceItemResults = GlobalElementStatements.Select(t => new ElementPriceItemResult
                {
                    BuyPrice = t.BuyPrice,
                    SellPrice = t.OrderPrice,
                    ElementId = t.Element._id
                }).ToList()
            };

            // Update element prices event
            var eventAuction = new UpdateElementPricesEvent {ElementPricedData = container.ToBson()};
            var eventAuctionData = new EventData((byte) EventCode.UpdateElementPrices, eventAuction);
            eventAuctionData.SendTo(_subscribers, new SendParameters());
        }

        private GlobalElementStatement Get(string elementId)
        {
            return _globalElementStatements.FirstOrDefault(t => t.Element._id == elementId);
        }

        #endregion

        #region Implementation of IRuntimeService

        public void AddSubscriber(PeerBase peerBase)
        {
            _fiber.Enqueue(() => OnAddSubscriber(peerBase));
        }

        public void RemoveSubscriber(PeerBase peerBase)
        {
            _fiber.Enqueue(() => OnRemoveSubscriber(peerBase));
        }

        /// <summary>
        /// </summary>
        /// <param name="subscriber"></param>
        private void OnAddSubscriber(PeerBase subscriber)
        {
            _subscribers.Add(subscriber);
        }

        /// <summary>
        /// </summary>
        /// <param name="subscriber"></param>
        private void OnRemoveSubscriber(PeerBase subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        #endregion
    }
}