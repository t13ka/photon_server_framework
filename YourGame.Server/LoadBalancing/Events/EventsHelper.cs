using ExitGames.Logging;

using MongoDB.Bson;

using Photon.SocketServer;

using YourGame.Server.Events.Economy;
using YourGame.Server.MasterServer;

namespace YourGame.Server.Events
{
    using YourGame.Common;

    public static class EventsHelper
    {
        public static void SendCreateElement()
        {
        }

        public static void SendMoveElement()
        {
        }

        public static void SendUpdateCurrency(this MasterClientPeer peer)
        {
            // Update profile event
            var currentPlayer = peer.GetCurrentPlayer();
            var changeCurrencyEvent =
                new UpdateCurrencyEvent
                    {
                        Gold = currentPlayer.Gold,
                        Crystal = currentPlayer.Crystals,
                        Keys = currentPlayer.Keys,
                        HealBox = currentPlayer.HealBox
                    };
            var currencyEvenData = new EventData((byte)EventCode.UpdateCurrency, changeCurrencyEvent);
            peer.SendEvent(currencyEvenData, new SendParameters());
        }

        public static void SendUpdatePlayerProfileEvent(this MasterClientPeer peer)
        {
            // Update profile event
            var currentPlayer = peer.GetCurrentPlayer();
            var profileEvent = new UpdateProfileEvent { ProfileData = currentPlayer.ToBson() };
            var profileEventData = new EventData((byte)EventCode.PlayerProfile, profileEvent);
            peer.SendEvent(profileEventData, new SendParameters());
        }

        public static void SendDomainConfigurationEvent(this MasterClientPeer peer)
        {
            var domainConfigBytes = MasterApplication.DomainConfiguration.ToBson();
            LogManager.GetCurrentClassLogger().Warn($"DomainConfigBytes: {domainConfigBytes.Length}");
            var e = new UpdateDomainConfigurationEvent { DomainConfiguration = domainConfigBytes };
            var eventData = new EventData((byte)EventCode.UpdateDomainConfiguration, e);
            peer.SendEvent(eventData, new SendParameters());
        }
    }
}