# JetBlack.JsonConsolerLogger

## Overview

This library provides a JSON console logger for the `Microsoft.Extensions.Logging` framework.

It was originally written for .Net services running in docker to provide structured logging for use with Elasticsearch.

## Issues

At present the public interface provided by the Microsoft logging framework passes an important parameter as an internal class. Reflection is used in order to access this parameters. **If the underlying implementation is changed this logger will break.**

## Installation

The package can be installed from [nuget](https://www.nuget.org/packages/JetBlack.JsonConsoleLogger/).

## Usage

This is a modification of the logger in `Microsoft.Extensions.Logging.Console`, and can be used
in exactly the same way. Please look at the [Microsoft documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.0) for more examples.

Here is an example using a logger factory.

```cs
using JetBlack.JsonConsoleLogger;

namespace Example
{
    class Program
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConfiguration(configuration.GetSection("Logging"))
                .AddJsonConsole();
        });

        var logger = loggerFactory.CreateLogger<Program>();

        logger.LogInformation("This is an {LevelName} message with a {Date}", "INFO", DateTime.Now);

        loggerFactory.Dispose();
    }
}
```

Here is a basic `appsettings.json` file.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "System": "Information",
      "Microsoft": "Information"
    },
    "JsonConsoleLogger": {
      "Timestamp": "utc"
    }
  }
}
```

This would produce the following output.

```bash
{"name":"Example.Program","level":"information","message":"This is an INFO message with a 09/30/2019 14:42:48","parameters":{"LevelName":"INFO","Date":"2019-09-30T14:42:48.2281211+01:00"},"timestamp":"2019-09-30T13:42:48.2402817"}
```

Each message is newline terminated.

## Configuration

### Timestamps

The `Timestamp` option can be one of: `none`, `local`, `utc`.

### Scopes

To include scope information set `IncludeScopes` to `true`.

### Logging to standard error

If `LogToStdErr` is `true` the output will be sent to stderr, otherwise it will go to stdout.

### Disabling exception logging

If `LogExceptions` is set to `false` the exception messages will not be output.

### Flattening exceptions

By default inner exceptions will be output as a nested dictionary. Setting
`flattenExceptions` to `true` changes this to a list with the outermost
at the start of the list and the innermost at the end.

### Tag names

Tags names can be overridden with a `names` dictionary.

Here is an example:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "System": "Information",
      "Microsoft": "Information"
    },
    "JsonConsole":
    {
      "Timestamp": "utc",
      "logToStdErr": true,
      "names": {
        "name": "logName",
        "level": "logLevel",
        "message": "logMessage",
        "parameters": "logParameters",
        "exception": "logException",
        "timestamp": "logTimestamp",
        "exceptionMessage": "logExceptionMessage",
        "innerException": "logInnerException",
        "stackTrace": "logStackTrace",
        "lineNumber": "logLineNumber",
        "columnNumber": "logColumnNumber",
        "fileName": "logFileName",
        "method": "logMethod",
        "trace": "trce",
        "debug": "dbug",
        "information": "info",
        "Warning": "warn",
        "error": "fail", 
        "critical": "crit"
      }
    }
  }
}
```
