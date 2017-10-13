namespace Warsmiths.Server.LoadShedding
{
    public interface ILatencyMonitor
    {
        int AverageLatencyMs { get; }
    }
}