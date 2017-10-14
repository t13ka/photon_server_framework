namespace YourGame.Common
{
    public enum EventCode : byte
    {
        PlayerProfile = 0,
        Notification = 1,
        UpdateCharacterList = 2,
        UpdateInventory = 3,
        UpdateEquipment = 4,
        UpdateAuction = 5,
        UpdateElementsOrder = 6,
        UpdateElementPrices = 7,
        UpdateDomainConfiguration = 8,
        GetRecieptEvent = 9,
        UpdateCommonCharacterProfile = 10,
        VictoryPrizes = 11,
        UpdateCurrency = 12,
        GameList = 230,
        GameListUpdate = 229,
        QueueState = 228,
        AppStats = 226,
        GameServerOffline = 225,
        Craft = 20,
    }
}