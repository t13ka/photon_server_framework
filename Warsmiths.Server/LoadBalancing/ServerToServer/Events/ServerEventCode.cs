namespace Warsmiths.Server.ServerToServer.Events
{
    public enum ServerEventCode
    {
        ////InitInstance = 0, 
        UpdateServer = 1,
        UpdateAppStats = 2,
        UpdateGameState = 3,
        RemoveGameState = 4,
        AuthenticateUpdate = 10
    }
}