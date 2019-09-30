#nullable enable

using System.Collections.Generic;

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
        public bool IncludeScopes { get; set; } = false;
        public bool LogToStdErr { get; set; } = false;
        public TimestampStyle Timestamp { get; set; } = TimestampStyle.Utc;
        public IDictionary<string, string?>? Names { get; set; } = null;
        public bool LogExceptions { get; set; } = true;
        public bool FlattenExceptions { get; set; } = false;
    }
}