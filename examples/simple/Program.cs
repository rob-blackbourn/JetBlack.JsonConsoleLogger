using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using JetBlack.JsonConsoleLogger;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(configuration.GetSection("Logging"))
                    .AddJsonConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogTrace("This is a {LevelName} message", "TRACE");
            logger.LogDebug("This is a {LevelName} message", "DEBUG");
            logger.LogInformation("This is an {LevelName} message with a {Date}", "INFO", DateTime.Now);
            logger.LogWarning("This is a {LevelName} message", "WARN");

            try
            {
                NestedThrow1("foo", 42);
            }
            catch (Exception error)
            {
                logger.LogError(error, "This is an {LevelName} message", "ERROR");
            }

            loggerFactory.Dispose();
        }

        static void NestedThrow1(string arg1, int arg2)
        {
            NestedThrow2(2.3);
        }

        static void NestedThrow2(double arg3)
        {
            throw new ApplicationException("This is not a test");
        }
    }
}
