namespace Warsmiths.Server.LoadShedding
{
    internal interface IFeedbackControlSystem
    {
        FeedbackLevel Output { get; }

        void SetPeerCount(int peerCount);

        void SetCpuUsage(int cpuUsage);

        void SetBusinessLogicQueueLength(int businessLogicQueue);

        void SetENetQueueLength(int enetQueue);

        void SetBandwidthUsage(int bytes);

        void SetLatencyTcp(int averageLatencyMs);

        void SetLatencyUdp(int averageLatencyMs);

        void SetOutOfRotation(bool isOutOfRotation);

        void SetEnetThreadsProcessing(int enetThreadsProcessing); 

        void SetTimeSpentInServer(int timeSpentInServer);

        void SetDisconnectRateUdp(int udpDisconnectRate);
    }
}