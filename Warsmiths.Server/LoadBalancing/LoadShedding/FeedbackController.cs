using System.Collections.Generic;
using ExitGames.Logging;

namespace Warsmiths.Server.LoadShedding
{
    internal class FeedbackController
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<FeedbackLevel, int> _thresholdValues;

        private int currentInput;

        public FeedbackController(
            FeedbackName feedbackName, Dictionary<FeedbackLevel, int> thresholdValues, int initialInput,
            FeedbackLevel initalFeedbackLevel)
        {
            this._thresholdValues = thresholdValues;
            FeedbackName = feedbackName;
            Output = initalFeedbackLevel;
            currentInput = initialInput;
        }

        public FeedbackName FeedbackName { get; }

        public FeedbackLevel Output { get; private set; }

        public FeedbackLevel GetNextHigherThreshold(FeedbackLevel level, out int result)
        {
            var next = level;
            while (next != FeedbackLevel.Highest)
            {
                next = FeedbackLevelOrder.GetNextHigher(next);
                if (_thresholdValues.TryGetValue(next, out result))
                {
                    return next;
                }
            }

            _thresholdValues.TryGetValue(level, out result);
            return level;
        }

        public FeedbackLevel GetNextLowerThreshold(FeedbackLevel level, out int result)
        {
            var next = level;
            while (next != FeedbackLevel.Lowest)
            {
                next = FeedbackLevelOrder.GetNextLower(next);
                if (_thresholdValues.TryGetValue(next, out result))
                {
                    return next;
                }
            }

            _thresholdValues.TryGetValue(level, out result);
            return level;
        }

        public bool SetInput(int input)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("SetInput: {0} value {1}", FeedbackName, input);
            }

            if (input > currentInput)
            {
                int threshold;
                var last = Output;
                var next = GetNextHigherThreshold(last, out threshold);
                while (next != last)
                {
                    if (input >= threshold)
                    {
                        last = next;
                        next = GetNextHigherThreshold(last, out threshold);
                    }
                    else
                    {
                        next = last;
                    }
                }

                currentInput = input;
                if (next != Output)
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat("Transit {0} from {1} to {2} with input {3}", FeedbackName, Output, next, input);
                    }

                    Output = next;
                    return true;
                }
            }
            else if (input < currentInput)
            {
                int threshold;
                var last = Output;
                var next = GetNextLowerThreshold(last, out threshold);
                while (next != last)
                {
                    if (input <= threshold)
                    {
                        last = next;
                        next = GetNextLowerThreshold(last, out threshold);
                    }
                    else
                    {
                        next = last;
                    }
                }

                currentInput = input;
                if (next != Output)
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat("Transit {0} from {1} to {2} with input {3}", FeedbackName, Output, next, input);
                    }

                    Output = next;
                    return true;
                }
            }

            return false;
        }
    }
}