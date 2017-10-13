namespace Warsmiths.Server.LoadShedding
{
    internal enum FeedbackName
    {
        CpuUsage, 
        BusinessLogicQueueLength, 
        ENetQueueLength, 
        PeerCount, 
        Bandwidth,
        LatencyTcp,
        LatencyUdp,
        OutOfRotation,
        EnetThreadsProcessing,
        TimeSpentInServer,
        DisconnectRateUdp
    }
}