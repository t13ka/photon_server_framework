using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using MongoDB.Bson;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.ListContainer;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.DataBaseService;
using Warsmiths.Server.Framework.Services;

namespace Warsmiths.Server.Services.Auction
{
    public sealed class AuctionRuntimeService : IRuntimeService, IDisposable
    {
        #region Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly HashSet<PeerBase> _subscribers = new HashSet<PeerBase>();

        /// <summary>
        /// </summary>
        private readonly PoolFiber _fiber;

        /// <summary>
        /// </summary>
        private static IRepository<Lot> _lotsService;

        #endregion

        #region Methods

        public AuctionRuntimeService(IRepository<Lot> repository)
        {
            _log.Debug(1);
            _lotsService = repository;
            _log.Debug(2);
            _fiber = new PoolFiber();
            _log.Debug(3);
            _fiber.Start();
            _log.Debug(4);
        }

        public void AddSubscriber(PeerBase peer)
        {
            _fiber.Enqueue(() => OnAddSubscriber(peer));
        }

        public void RemoveSubscriber(PeerBase peer)
        {
            _fiber.Enqueue(() => OnRemoveSubscriber(peer));
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


        /// <summary>
        /// </summary>
        /// <param name="lot"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Publish(Lot lot)
        {
            _lotsService.Create(lot);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat($"Publish: lot {lot._id}, owner:{lot.OwnerId}");
            }
        }

        public void Unpublish(Lot lot)
        {
            _lotsService.Delete(lot._id);
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat($"Unpublish: lot {lot._id}");
            }
        }

        public Lot GetLotByEquipmentId(string equipmentId)
        {
            return _lotsService.GetById(equipmentId);
        }

        public IList<Lot> GetAll()
        {
            return _lotsService.GetAll();
        }

        public long Count()
        {
            return _lotsService.Count();
        }

        public void Dispose()
        {
            _fiber.Stop();
            _fiber.Dispose();
        }

        public void SendUpdateAuctionDataToSubscribers()
        {
            var lotsContainer = new LotsListContainer {Lots = GetAll().ToList()};
            var data = lotsContainer.ToBson();

            // Update auction event
            var eventAuction = new UpdateAuctionEvent {AuctionData = data};
            var eventAuctionData = new EventData((byte) EventCode.UpdateAuction, eventAuction);
            eventAuctionData.SendTo(_subscribers, new SendParameters());
        }

        #endregion
    }
}