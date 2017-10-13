using ExitGames.Diagnostics.Counter;
using ExitGames.Diagnostics.Monitoring;

namespace Warsmiths.Server.Framework.Diagnostics
{
    public static class Counter
    {
        [PublishCounter("Games")]
        public static readonly NumericCounter Games = new NumericCounter("Games");
    }
}