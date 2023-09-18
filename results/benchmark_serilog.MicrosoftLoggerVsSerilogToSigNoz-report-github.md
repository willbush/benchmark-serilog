```

BenchmarkDotNet v0.13.8, NixOS 23.11 (Tapir)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host]     : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2
  Job-MASZPO : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2

Toolchain=.NET 8.0  

```
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
