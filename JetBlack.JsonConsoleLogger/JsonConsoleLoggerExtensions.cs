#nullable enable

using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace JetBlack.JsonConsoleLogger
{
    public static class JsonConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddJsonConsole(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, JsonConsoleLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<JsonConsoleLoggerOptions, JsonConsoleLoggerProvider>(builder.Services);
            return builder;
        }

        public static ILoggingBuilder AddJsonConsole(this ILoggingBuilder builder, Action<JsonConsoleLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddJsonConsole();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
