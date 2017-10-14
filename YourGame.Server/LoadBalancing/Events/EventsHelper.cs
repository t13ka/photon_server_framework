using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ExitGames.Logging;
using MongoDB.Bson;
using Photon.SocketServer;

using YourGame.Server.Events.Economy;
using YourGame.Server.MasterServer;
using YourGame.Server.Services.Economic;

namespace YourGame.Server.Events
{
    using YourGame.Common;
    using YourGame.Common.Domain;
    using YourGame.Common.Domain.Elements;
    using YourGame.Common.Domain.VictoryPrizes;
    using YourGame.Common.ListContainer;
    using YourGame.Server.Framework.Services;

    public static class EventsHelper
    {

        //
        public static void SendCreateElement()
        {
            
        }
        public static void SendMoveElement()
        {

        }

        public static void SendUpdatePlayerInventoryEvent(this MasterClientPeer peer)
        {
            // Update inventory event
            var currentPlayer = peer.GetCurrentPlayer();
            var e = new UpdateInventoryEvent {InventoryData = currentPlayer.Inventory.ToBson()};
            var eventData = new EventData((byte) EventCode.UpdateInventory, e);

            peer.SendEvent(eventData, new SendParameters());
        }
        public static void SendUpdateCurrency(this MasterClientPeer peer)
        {
            // Update profile event
            var currentPlayer = peer.GetCurrentPlayer();
            var changeCurrencyEvent = new UpdateCurrencyEvent { Gold = currentPlayer.Gold, Crystal = currentPlayer.Crystals, Keys = currentPlayer.Keys, HealBox = currentPlayer.HealBox};
            var currencyEvenData = new EventData((byte)EventCode.UpdateCurrency, changeCurrencyEvent);
            peer.SendEvent(currencyEvenData, new SendParameters());
        }
        public static void SendUpdatePlayerProfileEvent(this MasterClientPeer peer)
        {
            // Update profile event
            var currentPlayer = peer.GetCurrentPlayer();
            var profileEvent = new UpdateProfileEvent {ProfileData = currentPlayer.ToBson()};
            var profileEventData = new EventData((byte) EventCode.PlayerProfile, profileEvent);
            peer.SendEvent(profileEventData, new SendParameters());
        }

        public static void SendUpdateAuctionEvent(this MasterClientPeer peer, LotsListContainer container)
        {
            // Update inventory event
            var e = new UpdateAuctionEvent {AuctionData = container.ToBson()};
            var eventData = new EventData((byte) EventCode.UpdateAuction, e);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendUpdateCharactersList(this MasterClientPeer peer)
        {
            var currentPlayer = peer.GetCurrentPlayer();
            var container = new CharactersListContainer {Characters = currentPlayer.Characters};
            var e = new UpdateCharactersListEvent
            {
                CharactersListData = container.ToBson()
            };
            var eventData = new EventData((byte) EventCode.UpdateCharacterList, e);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendUpdateElementsOrderEvent(this MasterClientPeer peer)
        {
            var elementsService = ServiceManager.Get<EconomicRuntimeService>();
            var elements = elementsService.ElementStatements.Where(t => t.PlayerId == peer.UserId);
            var container = new ElementOrderListContainer
            {
                ElementOrderItemResults = elements.Select(t => new ElementOrderItemResult
                {
                    ElementId = t.ElementId,
                    Generated = t.Generated,
                    SecondsToFinishOrder = t.SecondsToFinishOrder,
                    CreatedDateTime = t.CreatedDateTime,
                    OrderedQuantity = t.Quantity,
                    PlannedCompletionDateTime = t.PlannedCompletionDateTime,
                    TimeSpanToFinish = t.TimeSpanToFinish
                }).ToList()
            };

            foreach (var elementOrderItemResult in container.ElementOrderItemResults)
            {
                Debug.WriteLine(elementOrderItemResult.ToString());
            }

            // Update elements order event
            var e = new UpdateElementsOrderEvent {ElementsOrderData = container.ToBson()};
            var eventData = new EventData((byte) EventCode.UpdateElementsOrder, e);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendUpdateEquipmentEvent(this MasterClientPeer peer)
        {
            var currentPlayer = peer.GetCurrentPlayer();

            var container = new EquipmentListContainer {Equipments = currentPlayer.GetCurrentCharacter().Equipment};
            // Update equipment event
            var eventEquipment = new UpdateEquipmentEvent
            {
                EquipmentData = container.ToBson()
            };
            var eventData = new EventData((byte) EventCode.UpdateEquipment, eventEquipment);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendElementPricesEvent(this MasterClientPeer peer, ElementsPricesContainer container)
        {
            // Update equipment event
            var eventUpdateElementPrices = new UpdateElementPricesEvent
            {
                ElementPricedData = container.ToBson()
            };
            var eventData = new EventData((byte)EventCode.UpdateElementPrices, eventUpdateElementPrices);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendDomainConfigurationEvent(this MasterClientPeer peer)
        {
            var domainConfigBytes = MasterApplication.DomainConfiguration.ToBson();
            LogManager.GetCurrentClassLogger().Warn($"DomainConfigBytes: {domainConfigBytes.Length}");
            var e = new UpdateDomainConfigurationEvent { DomainConfiguration = domainConfigBytes };
            var eventData = new EventData((byte)EventCode.UpdateDomainConfiguration, e);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendRecieptData(this MasterClientPeer peer, BaseReciept reciept)
        {
            var e = new GetRecieptEvent { RecieptData = reciept.ToBson() };
            var eventData = new EventData((byte)EventCode.GetRecieptEvent, e);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendUpdateCommonCharacterProfileEvent(this MasterClientPeer peer)
        {
            var currentPlayer = peer.GetCurrentPlayer();
            var currentCharacter = currentPlayer.GetCurrentCharacter();
            var e = new UpdateCommonCharacterProfileEvent { CommonCharacterProfile = currentCharacter.CommonProfile.ToBson() };
            var eventData = new EventData((byte)EventCode.UpdateCommonCharacterProfile, e);
            peer.SendEvent(eventData, new SendParameters());
        }

        public static void SendVictoryPrizes(this MasterClientPeer peer, VictoryPrizesResult prizes)
        {
            var e = new VictoryPrizesEvent {Prizes = prizes.ToBson()};

            var eventData = new EventData((byte)EventCode.VictoryPrizes, e);
            peer.SendEvent(eventData, new SendParameters());
        }
    }
}