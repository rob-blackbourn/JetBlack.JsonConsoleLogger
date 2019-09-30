#nullable enable

using System;
using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JetBlack.JsonConsoleLogger
{
    [ProviderAlias("JsonConsole")]
    public class JsonConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly IOptionsMonitor<JsonConsoleLoggerOptions> _options;
        private readonly ConcurrentDictionary<string, JsonConsoleLogger> _loggers;
        private readonly JsonConsoleLoggerProcessor _messageQueue;

        private IDisposable _optionsReloadToken;
        private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

        public JsonConsoleLoggerProvider(IOptionsMonitor<JsonConsoleLoggerOptions> options)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, JsonConsoleLogger>();

            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

            _messageQueue = new JsonConsoleLoggerProcessor();
            _messageQueue.Console = new LogConsole(new SystemConsole());
            _messageQueue.ErrorConsole = new LogConsole(new SystemConsole(stdErr: true));
        }

        private void ReloadLoggerOptions(JsonConsoleLoggerOptions options)
        {
            foreach (var logger in _loggers)
                logger.Value.Options = options;
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, loggerName => new JsonConsoleLogger(name, _messageQueue)
            {
                Options = _options.CurrentValue,
                ScopeProvider = _scopeProvider
            });
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
            _messageQueue.Dispose();
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;

            foreach (var logger in _loggers)
                logger.Value.ScopeProvider = _scopeProvider;
        }
    }
}