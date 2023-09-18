# benchmark-serilog

This repository contains performance benchmarks comparing Serilog logging
library to the default [.NET
logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line).

Specifically, I was interested in if Serilog added performance overhead when using
`Microsoft.Extensions.Logging.ILogger` in order to leverage:

- https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator
- https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging

Or should I just use Serilog's `Logger` directly?


## Optionally Setup SigNoz

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

To fix it, I `grep` for `max-file` in `signoz/deploy` and delete all `logging` yaml blocks:

```yaml
logging:
  options:
    max-size: 50m
    max-file: "3"
```

Visit http://localhost:3301/signup and create an account. Their setup appears to
mount and persist a sqlite file to store user name / password in their repo. So
if you forget your password, you can just remove signoz and `sudo git clean
-fxd` in the `signoz` repo.

## Run benchmarks

```sh
dotnet run -c Release
```

In NixOS, I had to specify the path to the dotnet executable for BenchmarkDotNet:

```sh
dotnet run -c Release -- --cli /etc/profiles/per-user/will/bin/dotnet
```

## Remove SigNoz

```sh
docker-compose -f docker/clickhouse-setup/docker-compose.yaml down
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

### Legends for charts below

- **Mean**      : Arithmetic mean of all measurements
- **Allocated** : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
- **1 ns**      : 1 Nanosecond (0.000000001 sec)

### Logger vs Serilog vs Serilog async console

Comparing default Microsoft Logger vs Serilog vs Serilog async console.

| Method       |       Mean | Allocated |
|--------------|-----------:|----------:|
| Default      | 6,827.7 ns |     192 B |
| Serilog      | 3,434.8 ns |    1048 B |
| SerilogAsync |   124.4 ns |     195 B |

Clearly using async `.WriteTo.Async(x => x.Console())` is a big win.

Benchmarks from now on will only compare using async.

### `Info` vs `Information`

Comparing source generated `Info` using Serilog via ILogger vs `Information`
method on Serilog's interface.

| Method      |     Mean | Allocated |
|-------------|---------:|----------:|
| CodeGenInfo | 379.9 ns |     867 B |
| Info        | 126.4 ns |     200 B |

### 3 ints

Comparing getting 3 ints into the body of the message and into the structured log.

| Method      |     Mean | Allocated |
|-------------|---------:|----------:|
| CodeGenInts | 732.9 ns |    1514 B |
| Ints        | 278.0 ns |     636 B |

### Point class with X / Y ints

Comparing getting a Point containing X / Y ints into the body of the message and
into the structured log.

| Method       |     Mean | Allocated |
|--------------|---------:|----------:|
| CodeGenPoint | 660.0 ns |    1.3 KB |
| Point        | 498.4 ns |   1.19 KB |

### AsyncOtelInfo vs AsyncConsoleInfo

Comparing Serilog async console sink vs Serilog async Open Telemetry to local
signoz.io

| Method          |     Mean | Allocated |
|-----------------|---------:|----------:|
| AsyncSigNozInfo | 128.2 ns |     229 B |
| AsyncInfo       | 125.7 ns |     209 B |

### MicrosoftLogger vs SerilogToSigNoz

Comparing Microsoft default logger to local SigNoz vs Serilog async to local SigNoz

| Method                    | Mean     | Allocated |
|-------------------------- |---------:|----------:|
| Default_OneString         | 145.6 ns |     164 B |
| Default_CodeGen_OneString | 227.8 ns |     277 B |
| Serilog_OneString         | 129.2 ns |     235 B |
| Serilog_CodeGen_OneString | 362.6 ns |    1019 B |
| Default_3Ints             | 328.4 ns |     428 B |
| Default_CodeGen_3Ints     | 419.3 ns |     552 B |
| Serilog_3Ints             | 266.8 ns |     713 B |
| Serilog_CodeGen_3Ints     | 679.8 ns |    1797 B |

## NoSinkLogger vs Serilog

Using no sink, compare Microsoft default logger to serilog.

| Method                    |       Mean | Allocated |
|---------------------------|-----------:|----------:|
| Default_OneString         |  11.622 ns |         - |
| Default_CodeGen_OneString |   1.683 ns |         - |
| Serilog_OneString         | 105.395 ns |     144 B |
| Serilog_CodeGen_OneString | 302.269 ns |     776 B |
| Default_3Ints             |  35.648 ns |     120 B |
| Default_CodeGen_3Ints     |   1.687 ns |         - |
| Serilog_3Ints             | 215.985 ns |     544 B |
| Serilog_CodeGen_3Ints     | 597.040 ns |    1392 B |
