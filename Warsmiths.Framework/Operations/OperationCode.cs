namespace Warsmiths.Server.Framework.Operations
{
    public enum OperationCode : byte
    {
        CustomOp = Warsmiths.Common.OperationCode.CustomOp,

        Join = Warsmiths.Common.OperationCode.Join,

        Leave = Warsmiths.Common.OperationCode.Leave,

        RaiseEvent = Warsmiths.Common.OperationCode.RaiseEvent,

        SetProperties = Warsmiths.Common.OperationCode.SetProperties,

        GetProperties = Warsmiths.Common.OperationCode.GetProperties,

        Ping = Warsmiths.Common.OperationCode.Ping,

        ChangeGroups = Warsmiths.Common.OperationCode.ChangeGroups,

        // operation codes in load the balancing project
        Authenticate = Warsmiths.Common.OperationCode.Authenticate,

        JoinLobby = Warsmiths.Common.OperationCode.JoinLobby,

        LeaveLobby = Warsmiths.Common.OperationCode.LeaveLobby,

        CreateGame = Warsmiths.Common.OperationCode.CreateGame,

        JoinGame = Warsmiths.Common.OperationCode.JoinGame,

        JoinRandomGame = Warsmiths.Common.OperationCode.JoinRandomGame,

        // CancelJoinRandomGame = 224, currently not used 
        DebugGame = Warsmiths.Common.OperationCode.DebugGame,

        FiendFriends = Warsmiths.Common.OperationCode.FiendFriends
    }
}