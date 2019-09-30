#nullable enable

using System.Collections.Generic;

namespace JetBlack.JsonConsoleLogger
{
    internal readonly struct LogEntry
    {
        public LogEntry(
            string name,
            string message,
            string? timeStamp,
            string levelString,
            IDictionary<string, object> parameters,
            object? exception,
            IList<string>? scopes,
            JsonConsoleLoggerOptions? options)
        {
            Name = name;
            TimeStamp = timeStamp;
            LevelString = levelString;
            Message = message;
            Parameters = parameters;
            Exception = exception;
            Scopes = scopes;
            Options = options;
        }

        public readonly string Name;
        public readonly string? TimeStamp;
        public readonly string LevelString;
        public readonly string Message;
        public readonly IDictionary<string, object>? Parameters;
        public readonly object? Exception;
        public readonly IList<string>? Scopes;
        public readonly JsonConsoleLoggerOptions? Options;
    }
}