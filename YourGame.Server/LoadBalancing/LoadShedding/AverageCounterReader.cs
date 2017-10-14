using System.Linq;
using ExitGames.Diagnostics.Counter;

namespace YourGame.Server.LoadShedding
{
    internal sealed class AverageCounterReader : PerformanceCounterReader
    {
        private readonly ValueHistory values;

        public AverageCounterReader(int capacity, string categoryName, string counterName)
            : base(categoryName, counterName)
        {
            values = new ValueHistory(capacity);
        }

        public AverageCounterReader(int capacity, string categoryName, string counterName, string instanceName)
            : base(categoryName, counterName, instanceName)
        {
            values = new ValueHistory(capacity);
        }

        public double GetNextAverage()
        {
            var value = GetNextValue();
            values.Add((int) value);
            return values.Average();
        }
    }
}