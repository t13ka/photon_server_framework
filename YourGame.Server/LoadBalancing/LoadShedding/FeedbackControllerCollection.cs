using System.Collections.Generic;
using System.Linq;

namespace YourGame.Server.LoadShedding
{
    internal sealed class FeedbackControllerCollection
    {
        private readonly Dictionary<FeedbackName, FeedbackController> values;

        public FeedbackControllerCollection(params FeedbackController[] controller)
        {
            values = new Dictionary<FeedbackName, FeedbackController>(controller.Length);
            Output = FeedbackLevel.Lowest;
            foreach (var c in controller)
            {
                values.Add(c.FeedbackName, c);
                if (c.Output > Output)
                {
                    Output = c.Output;
                }
            }
        }

        public FeedbackLevel Output { get; private set; }

        public FeedbackLevel CalculateOutput()
        {
            return Output = values.Values.Max(controller => controller.Output);
        }

        public FeedbackLevel SetInput(FeedbackName key, int input)
        {
            // Controllers are optional, we don't need to configure them all. 
            FeedbackController controller;
            if (values.TryGetValue(key, out controller))
            {
                if (controller.SetInput(input))
                {
                    if (controller.Output > Output)
                    {
                        return Output = controller.Output;
                    }

                    if (controller.Output < Output)
                    {
                        return CalculateOutput();
                    }
                }
            }

            return Output;
        }
    }
}