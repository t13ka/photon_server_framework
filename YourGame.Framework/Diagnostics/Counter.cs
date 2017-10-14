namespace YourGame.Server.Framework.Diagnostics
{
    using ExitGames.Diagnostics.Counter;
    using ExitGames.Diagnostics.Monitoring;

    public static class Counter
    {
        [PublishCounter("Games")]
        public static readonly NumericCounter Games = new NumericCounter("Games");
    }
}