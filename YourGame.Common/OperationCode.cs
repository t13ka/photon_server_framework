﻿namespace YourGame.Common
{
    public enum OperationCode
    {
        // --- These codes work in conjunction with customop
        CustomOp = 0,

        Registration = 1,

        Login = 2,

        GetProfile = 3,

        CreateCharacter = 4,

        RemoveCharacter = 5,

        TryWearEquipment = 6,

        TryUnwearEquipment = 7,

        GetLots = 8,

        TryBuyLot = 9,

        PublishLot = 10,

        UnpublishLot = 11,

        SelectCharacter = 12,

        GetInventory = 13,

        AddToInventory = 14,

        DeleteFromInventory = 15,

        DestroyEquipment = 17,

        CreateArmor = 18,

        EnchantEquipment = 19,

        SellElement = 20,

        OrderElement = 21,

        GetElementsOrders = 22,

        ReceiveElement = 23,

        SaveReservedFieldsForCharacter = 24,

        Logout = 25,

        TakeDestroyResult = 26,

        GetElementPrices = 27,

        SaveReciept = 28,

        GetReciept = 29,

        InsertModule = 30,

        RemoveModule = 31,

        AddClass = 33,

        RemoveClass = 34,

        AddAbility = 35,

        RemoveAbility = 36,

        SetCraftExperience = 37,

        SetExperience = 38,

        SetSkillPercentValue = 39,

        RequestVictoryPrizes = 40,

        SelectVictoryPrizes = 41,

        // Craft
        GetRecieptStage = 42,

        GetRecieptInfo = 43,

        GetReciepts = 44,

        CraftQuestComplete = 45,

        GetAllReciepts = 46,

        CraftQuestNext = 47,

        StartReciept = 48,

        EndReciept = 49,

        LastRecieptQuest = 50,

        ChangeCurrency = 51,

        UpdateCurrency = 52,

        // Combats
        IncreaseMaxAllyForce = 70,

        // Tasks
        CompleteTask = 99,

        TaskStarted = 100,

        // -------------------- do not modify this -----------------
        Join = 255,

        Leave = 254,

        RaiseEvent = 253,

        SetProperties = 252,

        GetProperties = 251,

        Ping = 249,

        ChangeGroups = 248,

        // operation codes in load the balancing project
        Authenticate = 230,

        JoinLobby = 229,

        LeaveLobby = 228,

        CreateGame = 227,

        JoinGame = 226,

        JoinRandomGame = 225,

        // CancelJoinRandomGame = 224, currently not used 
        DebugGame = 223,

        FiendFriends = 222,
    }
}