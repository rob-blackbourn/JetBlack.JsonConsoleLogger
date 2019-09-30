#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using Newtonsoft.Json;

namespace JetBlack.JsonConsoleLogger
{
    internal class JsonConsoleLoggerProcessor : IDisposable
    {
        private const int MaxQueuedMessages = 1024;

        private readonly BlockingCollection<LogEntry> _messageQueue = new BlockingCollection<LogEntry>(MaxQueuedMessages);
        private readonly Thread _outputThread;

        public IConsole? Console;
        public IConsole? ErrorConsole;

        public JsonConsoleLoggerProcessor()
        {
            // Start Console message queue processor
            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "JSON Console logger queue processing thread"
            };
            _outputThread.Start();
        }

        public virtual void EnqueueMessage(LogEntry message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

            // Adding is completed so just log the message
            try
            {
                WriteMessage(message);
            }
            catch (Exception) { }
        }

        internal void WriteMessage(LogEntry logEntry)
        {
            if (!TryWriteMessage(logEntry, true))
                TryWriteMessage(logEntry, false);
        }

        private bool TryWriteMessage(LogEntry logEntry, bool includeParameters)
        {
            try
            {
                var console = logEntry.Options?.LogToStdErr == true ? ErrorConsole : Console;

                var body = new Dictionary<string, object?>
            {
                { logEntry.Options.GetName(Names.Name), logEntry.Name},
                { logEntry.Options.GetName(Names.Level), logEntry.LevelString},
                { logEntry.Options.GetName(Names.Message), logEntry.Message}
            };

                if (includeParameters)
                    body[logEntry.Options.GetName(Names.Parameters)] = logEntry.Parameters;
                else
                    body[logEntry.Options.GetName(Names.Parameters)] = "failed to write parameters";

                if (logEntry.Exception != null)
                    body[logEntry.Options.GetName(Names.Exception)] = logEntry.Exception;
                if (logEntry.TimeStamp != null)
                    body[logEntry.Options.GetName(Names.Timestamp)] = logEntry.TimeStamp;
                if (logEntry.Scopes != null)
                    body[logEntry.Options.GetName(Names.Scopes)] = logEntry.TimeStamp;

                var message = JsonConvert.SerializeObject(body);

                console?.WriteLine(message);
                console?.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(message);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _outputThread.Join(1500); // with timeout in-case Console is locked by user input
            }
            catch (ThreadStateException) { }
        }
    }
}
