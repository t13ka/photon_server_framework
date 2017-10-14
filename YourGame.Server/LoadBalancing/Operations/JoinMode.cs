namespace YourGame.Server.Operations
{
    public enum JoinMode : byte
    {
        Default = 0,
        CreateIfNotExists = 1,
        Rejoin = 2
    }
}