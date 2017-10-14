namespace YourGame.Server.Framework.Diagnostics.OperationLogging
{
    using System;

    public class LogEntry
    {
        public LogEntry(DateTime utcCreated, string action, string message)
        {
            Action = action;
            UtcCreated = utcCreated;
            Message = message;
        }

        public LogEntry(string action, string message)
            : this(DateTime.UtcNow, action, message)
        {
        }

        public DateTime UtcCreated;

        public string Action;

        public string Message;

        public override string ToString()
        {
            return string.Format("{0} - {1}: {2}", UtcCreated, Action, Message);
        }
    }
}