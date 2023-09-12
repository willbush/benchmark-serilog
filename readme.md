# benchmark-serilog

## Console Benchmarks Summary

```
BenchmarkDotNet v0.13.8, NixOS 23.11 (Tapir)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 7.0.400
  [Host]     : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
  Job-XBVYPE : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2

Toolchain=.NET 7.0
```


| Method                          |       Mean |     Error |    StdDev |     Median |   Gen0 |   Gen1 | Allocated |
|---------------------------------|-----------:|----------:|----------:|-----------:|-------:|-------:|----------:|
| Default                         | 8,348.1 ns | 188.06 ns | 548.58 ns | 8,506.3 ns |      - |      - |     208 B |
| Serilog_CodeGen_Point           | 8,459.4 ns |  69.09 ns |  53.94 ns | 8,465.6 ns |      - |      - |     384 B |
| Serilog                         | 4,130.8 ns | 115.36 ns | 340.14 ns | 4,268.5 ns | 0.0076 |      - |    1048 B |
| Serilog_Async                   |   158.3 ns |   4.16 ns |  12.21 ns |   161.0 ns | 0.0024 |      - |     201 B |
| Default_CodeGen_Something       | 7,833.9 ns | 246.42 ns | 718.83 ns | 7,923.0 ns |      - |      - |     352 B |
| Serilog_CodeGen_Something       | 3,525.1 ns | 157.46 ns | 464.28 ns | 3,392.1 ns | 0.0229 |      - |    2072 B |
| Serilog_CodeGen_Something_Async |   751.9 ns |   3.08 ns |   2.57 ns |   752.5 ns | 0.0153 | 0.0134 |    1308 B |
| Default_CodeGen_Info            | 7,964.3 ns | 232.66 ns | 686.01 ns | 7,949.6 ns |      - |      - |     312 B |
| Serilog_CodeGen_Info            | 3,798.8 ns | 166.70 ns | 488.89 ns | 3,857.7 ns | 0.0153 |      - |    1680 B |
| Serilog_CodeGen_Info_Async      |   558.8 ns |   6.22 ns |   5.20 ns |   558.0 ns | 0.0105 | 0.0086 |     922 B |
| Serilog_Point                   | 4,063.3 ns | 180.59 ns | 523.93 ns | 4,030.7 ns | 0.0229 |      - |    2160 B |
| Serilog_Point_Async             | 1,345.7 ns |  26.61 ns |  69.63 ns | 1,357.8 ns | 0.0191 | 0.0191 |    1727 B |

### Warnings

```
MultimodalDistribution
  ConsoleBenchmarks.Serilog_Async: Toolchain=.NET 7.0        -> It seems that the distribution is bimodal (mValue = 3.24)
  ConsoleBenchmarks.Default_CodeGen_Info: Toolchain=.NET 7.0 -> It seems that the distribution is bimodal (mValue = 3.85)
  ConsoleBenchmarks.Serilog_CodeGen_Info: Toolchain=.NET 7.0 -> It seems that the distribution is bimodal (mValue = 3.35)
  ConsoleBenchmarks.Serilog_Point: Toolchain=.NET 7.0        -> It seems that the distribution is multimodal (mValue = 5.07)
```

### Hints

```
Outliers
  ConsoleBenchmarks.Default: Toolchain=.NET 7.0                         -> 2 outliers were removed, 6 outliers were detected (6.53 us..7.22 us, 9.54 us, 9.65 us)
  ConsoleBenchmarks.Serilog_CodeGen_Point: Toolchain=.NET 7.0           -> 3 outliers were removed (8.72 us..8.99 us)
  ConsoleBenchmarks.Serilog: Toolchain=.NET 7.0                         -> 1 outlier  was  detected (3.07 us)
  ConsoleBenchmarks.Serilog_Async: Toolchain=.NET 7.0                   -> 1 outlier  was  removed, 2 outliers were detected (120.14 ns, 188.22 ns)
  ConsoleBenchmarks.Default_CodeGen_Something: Toolchain=.NET 7.0       -> 2 outliers were removed, 11 outliers were detected (5.59 us..6.40 us, 9.55 us, 9.57 us)
  ConsoleBenchmarks.Serilog_CodeGen_Something_Async: Toolchain=.NET 7.0 -> 2 outliers were removed (764.36 ns, 811.14 ns)
  ConsoleBenchmarks.Serilog_CodeGen_Info: Toolchain=.NET 7.0            -> 1 outlier  was  removed (21.79 us)
  ConsoleBenchmarks.Serilog_CodeGen_Info_Async: Toolchain=.NET 7.0      -> 2 outliers were removed (583.47 ns, 583.49 ns)
  ConsoleBenchmarks.Serilog_Point: Toolchain=.NET 7.0                   -> 3 outliers were removed (5.73 us..6.04 us)
```

### Legends
```
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Median    : Value separating the higher half of all measurements (50th percentile)
  Gen0      : GC Generation 0 collects per 1000 operations
  Gen1      : GC Generation 1 collects per 1000 operations
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns      : 1 Nanosecond (0.000000001 sec)
```


## No Sink Benchmarks Summary

```
BenchmarkDotNet v0.13.8, NixOS 23.11 (Tapir)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 7.0.400
  [Host]     : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
  Job-XBVYPE : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2

Toolchain=.NET 7.0
```


| Method                    |       Mean |     Error |    StdDev |   Gen0 | Allocated |
|---------------------------|-----------:|----------:|----------:|-------:|----------:|
| Default                   |  11.229 ns | 0.0009 ns | 0.0008 ns |      - |         - |
| Serilog                   | 103.643 ns | 0.0574 ns | 0.0537 ns | 0.0017 |     144 B |
| Default_CodeGen_Something |   1.714 ns | 0.0005 ns | 0.0005 ns |      - |         - |
| Serilog_CodeGen_Something |   2.754 ns | 0.0438 ns | 0.0409 ns |      - |         - |
| Default_CodeGen_Info      |   1.704 ns | 0.0001 ns | 0.0001 ns |      - |         - |
| Serilog_CodeGen_Info      |   2.488 ns | 0.0098 ns | 0.0092 ns |      - |         - |
| Serilog_CodeGen_Point     |   2.399 ns | 0.0014 ns | 0.0012 ns |      - |         - |

### Hints

```
Outliers
  NoSinkBenchmarks.Default: Toolchain=.NET 7.0               -> 1 outlier  was  removed (12.37 ns)
  NoSinkBenchmarks.Default_CodeGen_Info: Toolchain=.NET 7.0  -> 1 outlier  was  removed (2.67 ns)
  NoSinkBenchmarks.Serilog_CodeGen_Info: Toolchain=.NET 7.0  -> 1 outlier  was  detected (3.40 ns)
  NoSinkBenchmarks.Serilog_CodeGen_Point: Toolchain=.NET 7.0 -> 1 outlier  was  removed (3.32 ns)
```

### Legends

```
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Gen0      : GC Generation 0 collects per 1000 operations
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns      : 1 Nanosecond (0.000000001 sec)
```
