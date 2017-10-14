namespace YourGame.Server.LoadShedding
{
    public interface ILatencyMonitor
    {
        int AverageLatencyMs { get; }
    }
}