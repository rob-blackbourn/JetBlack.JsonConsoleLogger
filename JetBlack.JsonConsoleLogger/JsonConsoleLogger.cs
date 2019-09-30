#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace JetBlack.JsonConsoleLogger
{
    internal class JsonConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly JsonConsoleLoggerProcessor _queueProcessor;

        internal JsonConsoleLogger(string? name, JsonConsoleLoggerProcessor? loggerProcessor)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (loggerProcessor == null)
                throw new ArgumentNullException(nameof(loggerProcessor));

            _name = name;
            _queueProcessor = loggerProcessor;
        }

        internal IExternalScopeProvider? ScopeProvider { get; set; }
        internal JsonConsoleLoggerOptions? Options { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));


            var message = formatter(state, exception);
            var parameters = GetParameters(state);

            if (!string.IsNullOrEmpty(message) || exception != null)
                WriteMessage(logLevel, _name, eventId.Id, message, parameters, exception);
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, IDictionary<string, object> parameters, Exception exception)
        {
            var entry = CreateDefaultLogMessage(logLevel, logName, eventId, message, parameters, exception);
            _queueProcessor.EnqueueMessage(entry);
        }

        private LogEntry CreateDefaultLogMessage(LogLevel logLevel, string logName, int eventId, string message, IDictionary<string, object> parameters, Exception exception)
        {
            var logLevelString = GetLogLevelString(logLevel);
            var scopes = GetScopeInformation();
            var exceptionDetails = GetExceptionDetails(exception);

            return new LogEntry(
                _name,
                message,
                GetFormattedTimestamp(),
                logLevelString,
                parameters,
                exceptionDetails,
                scopes,
                Options
            );
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;

        private string? GetFormattedTimestamp()
        {
            var style = Options?.Timestamp ?? TimestampStyle.None;
            if (style == TimestampStyle.None)
                return null;
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
        }

        private string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Options.GetName(Names.Trace);
                case LogLevel.Debug:
                    return Options.GetName(Names.Debug);
                case LogLevel.Information:
                    return Options.GetName(Names.Information);
                case LogLevel.Warning:
                    return Options.GetName(Names.Warning);
                case LogLevel.Error:
                    return Options.GetName(Names.Error);
                case LogLevel.Critical:
                    return Options.GetName(Names.Critical);
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private IDictionary<string, object> GetParameters(object? state)
        {
            if (state == null)
                return new Dictionary<string, object>();

            var formattedLogValues = new FormattedLogValues(state);
            return formattedLogValues
                .Where(x => x.Key != "{OriginalFormat}")
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<string, object?>? GetExceptionDetails(Exception? exception)
        {
            if (exception == null)
                return null;

            return new Dictionary<string, object?>
            {
                { Options.GetName(Names.ExceptionMessage), exception.Message },
                { Options.GetName(Names.StackTrace), GetStackTrace(exception) },
                { Options.GetName(Names.InnerException), GetExceptionDetails(exception.InnerException) }
            };
        }

        private Dictionary<string, object>[] GetStackTrace(Exception exception)
        {
            var stackTrace = new StackTrace(exception, true);

            var frames = new List<Dictionary<string, object>>();
            foreach (var frame in stackTrace.GetFrames())
            {
                frames.Add(
                    new Dictionary<string, object>
                    {
                        { Options.GetName(Names.LineNumber), frame.GetFileLineNumber() },
                        { Options.GetName(Names.ColumnNumber), frame.GetFileColumnNumber() },
                        { Options.GetName(Names.FileName), frame.GetFileName() },
                        { Options.GetName(Names.Method), frame.GetMethod().ToString() }
                    }
                );
            }

            return frames.ToArray();
        }

        private string[] GetScopeInformation()
        {
            var scopeProvider = ScopeProvider;
            if ((Options?.IncludeScopes ?? false) && scopeProvider != null)
            {
                var scopes = new List<string>();
                scopeProvider.ForEachScope((scope, state) =>
                {
                    scopes.Add(scope?.ToString() ?? state);
                }, (string.Empty));
                return scopes.ToArray();
            }
            else
                return new string[0];
        }
    }
}
