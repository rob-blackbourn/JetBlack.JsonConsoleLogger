#nullable enable

using System.Collections.Generic;

namespace JetBlack.JsonConsoleLogger
{
    internal readonly struct LogMessageEntry
    {
        public LogMessageEntry(
            string name,
            string message,
            string? timeStamp,
            string levelString,
            bool logAsError,
            IDictionary<string, object> parameters,
            IDictionary<string, object?>? exception)
        {
            Name = name;
            TimeStamp = timeStamp;
            LevelString = levelString;
            Message = message;
            LogAsError = logAsError;
            Parameters = parameters;
            Exception = exception;
        }

        public readonly string Name;
        public readonly string? TimeStamp;
        public readonly string LevelString;
        public readonly string Message;
        public readonly bool LogAsError;
        public readonly IDictionary<string, object>? Parameters;
        public readonly IDictionary<string, object?>? Exception;
    }
}