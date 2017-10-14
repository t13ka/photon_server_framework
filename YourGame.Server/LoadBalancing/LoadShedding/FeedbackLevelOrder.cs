using System.Collections.Generic;

namespace YourGame.Server.LoadShedding
{
    internal static class FeedbackLevelOrder
    {
        private static readonly Dictionary<FeedbackLevel, FeedbackLevel> ascending = new Dictionary<FeedbackLevel, FeedbackLevel> 
            {
                { FeedbackLevel.Lowest, FeedbackLevel.Low }, 
                { FeedbackLevel.Low, FeedbackLevel.Normal }, 
                { FeedbackLevel.Normal, FeedbackLevel.High }, 
                { FeedbackLevel.High, FeedbackLevel.Highest }, 
                { FeedbackLevel.Highest, FeedbackLevel.Highest },
            };

        private static readonly Dictionary<FeedbackLevel, FeedbackLevel> descending = new Dictionary<FeedbackLevel, FeedbackLevel> 
            {
                { FeedbackLevel.Lowest, FeedbackLevel.Lowest }, 
                { FeedbackLevel.Low, FeedbackLevel.Lowest }, 
                { FeedbackLevel.Normal, FeedbackLevel.Low }, 
                { FeedbackLevel.High, FeedbackLevel.Normal }, 
                { FeedbackLevel.Highest, FeedbackLevel.Normal },
            };

        public static FeedbackLevel GetNextHigher(FeedbackLevel level)
        {
            return ascending[level];
        }

        public static FeedbackLevel GetNextLower(FeedbackLevel level)
        {
            return descending[level];
        }
    }
}