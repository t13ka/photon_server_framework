namespace Warsmiths.Server.ServerToServer.Events
{
    public enum ServerParameterCode
    {
        UdpAddress = 10,
        TcpAddress = 11,

        PeerCount = 20,
        GameCount = 21,
        LoadIndex = 22,
        ServerState = 23,
        MaxPlayer = 24,
        IsOpen = 25,
        IsVisible = 26,
        LobbyPropertyFilter = 27,

        AuthList = 30,

        NewUsers = 40,
        RemovedUsers = 41,
        Reinitialize = 42
    }
}