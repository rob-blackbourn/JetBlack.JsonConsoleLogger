#nullable enable

using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace JetBlack.JsonConsoleLogger
{
    public enum TimestampStyle
    {
        None,
        Local,
        Utc
    }

    public class JsonConsoleLoggerOptions
    {
        public bool IncludeScopes { get; set; }
        public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.None;
        public TimestampStyle Timestamp { get; set; } = TimestampStyle.Utc;
        public IDictionary<string, string?>? Names { get; set; }
    }
}