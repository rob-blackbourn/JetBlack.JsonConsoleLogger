#nullable enable

namespace JetBlack.JsonConsoleLogger
{
    public static class Names
    {
        public const string Name = "name";
        public const string Level = "level";
        public const string Message = "message";
        public const string Parameters = "parameters";
        public const string Exception = "exception";
        public const string InnerException = "innerException";
        public const string Timestamp = "timestamp";
        public const string Trace = "trace";
        public const string Debug = "debug";
        public const string Information = "information";
        public const string Warning = "warning";
        public const string Error = "error";
        public const string Critical = "critical";
        public const string ExceptionMessage = "exceptionMessage";
        public const string StackTrace = "stackTrace";
        public const string FileName = "fileName";
        public const string LineNumber = "lineNumber";
        public const string ColumnNumber = "columnNumber";
        public const string Method = "method";
        public const string Scopes = "scopes";

        public static string GetName(this JsonConsoleLoggerOptions? options, string name)
        {
            if (options?.Names != null && options.Names.TryGetValue(name, out var value) && value != null)
                return value;
            return name;
        }
    }
}