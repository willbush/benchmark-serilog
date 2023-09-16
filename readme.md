# benchmark-serilog

This repository contains performance benchmarks comparing Serilog logging
library to the default [.NET
logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line).

Specifically, I was interested in if Serilog added performance overhead when using
`Microsoft.Extensions.Logging.ILogger` in order to leverage:

- https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator
- https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging

## Overview

Benchmarks different forms of logging, including:

- Default `ILogger` (Microsoft.Extensions.Logging.ILogger)
- Default Serilog `Logger`
- Serilog `Logger` via default `ILogger` using compile-time logging source generation

## Run benchmarks

```sh
dotnet run -c Release -- --filter "*"
```

type `*` and hit enter.

In NixOS, I had to specify the path to the dotnet executable for BenchmarkDotNet:

```sh
dotnet run -c Release -- --filter "*" --cli /etc/profiles/per-user/will/bin/dotnet
```

## Console Benchmarks Summary
