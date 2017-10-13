using System.Collections.Generic;
using System.Text;

using ExitGames.Logging;

namespace Warsmiths.Server.Framework.Diagnostics.OperationLogging
{
    public class LogQueue
    {
        public const int DefaultCapacity = 1000;

        #region Constructors and Destructors

        public LogQueue(string name, int capacity)
        {
            _capacity = capacity;
            _queue = new Queue<LogEntry>(capacity);
            _name = name;
        }

        #endregion

        #region Constants and Fields

        public readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly int _capacity;

        private readonly string _name;

        private readonly Queue<LogEntry> _queue;

        #endregion

        #region Public Methods

        public void Add(LogEntry value)
        {
            if (Log.IsDebugEnabled)
            {
                if (_queue.Count == _capacity)
                {
                    _queue.Dequeue();
                }

                _queue.Enqueue(value);
            }
        }

        public void WriteLog()
        {
            if (Log.IsDebugEnabled)
            {
                var logEntries = _queue.ToArray();
                var sb = new StringBuilder(logEntries.Length + 1);
                sb.AppendFormat("OperationLog for Game {0}:", _name).AppendLine();
                foreach (var entry in logEntries)
                {
                    sb.AppendFormat("{0}: {1}", _name, entry).AppendLine();
                }

                Log.Debug(sb.ToString());
            }
        }

        #endregion
    }
}