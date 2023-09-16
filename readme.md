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

Some benchmarks run using Open Telemetry against a local Signoz server. To set it up run:

```sh
git clone https://github.com/SigNoz/signoz.git
cd signoz/deploy
docker-compose -f docker/clickhouse-setup/docker-compose.yaml up -d
```
Personally, I get this error:

```
Error response from daemon: unknown log opt 'max-file' for journald log driver
```

To fix it, I grep for `max-file` in `signoz/deploy` all yaml blocks like, and delete them:

```yaml
logging:
  options:
    max-size: 50m
    max-file: "3"
```

Then run:

```sh
dotnet run -c Release -- --filter "*"
```

In NixOS, I had to specify the path to the dotnet executable for BenchmarkDotNet:

```sh
dotnet run -c Release -- --filter "*" --cli /etc/profiles/per-user/will/bin/dotnet
```

## Console Benchmarks Summary

```
BenchmarkDotNet v0.13.8, NixOS 23.11 (Tapir)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host]     : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2
  Job-YHBHMM : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2

Toolchain=.NET 8.0
```

### AsyncOtelInfo vs AsyncInfo


Comparing Serilog async console sink vs Serilog async Open Telemetry to local
signoz.io

| Method        | Mean     | Allocated |
|-------------- |---------:|----------:|
| AsyncOtelInfo | 132.3 ns |     240 B |
| AsyncInfo     | 136.2 ns |     201 B |

### Logger vs Serilog vs Serilog async console

Comparing default Microsoft Logger vs Serilog vs Serilog async console.

| Method       |       Mean | Allocated |
|--------------|-----------:|----------:|
| Default      | 6,716.0 ns |     208 B |
| Serilog      | 3,438.1 ns |    1048 B |
| SerilogAsync |   132.1 ns |     199 B |

Clearly using async `.WriteTo.Async(x => x.Console())` is a big win.

Benchmarks from now on will only compare using async.

### `Info` vs `Information`

Comparing source generated `Info` vs `Information` method on Serilog's interface.

| Method      | Mean     | Allocated |
|------------ |---------:|----------:|
| CodeGenInfo | 394.9 ns |     916 B |
| Info        | 128.7 ns |     193 B |

### 3 ints

Comparing getting 3 ints into the body of the message and into the structured log.

| Method      | Mean     | Allocated |
|------------ |---------:|----------:|
| CodeGenInts | 762.5 ns |    1607 B |
| Ints        | 282.1 ns |     647 B |

### Point class with X / Y ints

Comparing getting a Point containing X / Y ints into the body of the message and
into the structured log.

| Method       | Mean     | Allocated |
|------------- |---------:|----------:|
| CodeGenPoint | 636.6 ns |   1.29 KB |
| Point        | 647.4 ns |   1.26 KB |
