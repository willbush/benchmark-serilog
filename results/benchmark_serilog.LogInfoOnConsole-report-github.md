```

BenchmarkDotNet v0.13.8, NixOS 23.11 (Tapir)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host]     : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2
  Job-MASZPO : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2

Toolchain=.NET 8.0  

```
| Method       | Mean       | Allocated |
|------------- |-----------:|----------:|
| Default      | 6,827.7 ns |     192 B |
| Serilog      | 3,434.8 ns |    1048 B |
| SerilogAsync |   124.4 ns |     195 B |
