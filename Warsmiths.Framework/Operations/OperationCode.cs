namespace YourGame.Server.Framework.Operations
{
    public enum OperationCode : byte
    {
        CustomOp = YourGame.Common.OperationCode.CustomOp,

        Join = YourGame.Common.OperationCode.Join,

        Leave = YourGame.Common.OperationCode.Leave,

        RaiseEvent = YourGame.Common.OperationCode.RaiseEvent,

        SetProperties = YourGame.Common.OperationCode.SetProperties,

        GetProperties = YourGame.Common.OperationCode.GetProperties,

        Ping = YourGame.Common.OperationCode.Ping,

        ChangeGroups = YourGame.Common.OperationCode.ChangeGroups,

        // operation codes in load the balancing project
        Authenticate = YourGame.Common.OperationCode.Authenticate,

        JoinLobby = YourGame.Common.OperationCode.JoinLobby,

        LeaveLobby = YourGame.Common.OperationCode.LeaveLobby,

        CreateGame = YourGame.Common.OperationCode.CreateGame,

        JoinGame = YourGame.Common.OperationCode.JoinGame,

        JoinRandomGame = YourGame.Common.OperationCode.JoinRandomGame,

        // CancelJoinRandomGame = 224, currently not used 
        DebugGame = YourGame.Common.OperationCode.DebugGame,

        FiendFriends = YourGame.Common.OperationCode.FiendFriends
    }
}