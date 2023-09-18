```

BenchmarkDotNet v0.13.8, NixOS 23.11 (Tapir)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host]     : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2
  Job-MASZPO : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2

Toolchain=.NET 8.0  

```
| Method                    | Mean       | Allocated |
|-------------------------- |-----------:|----------:|
| Default_OneString         |  11.622 ns |         - |
| Default_CodeGen_OneString |   1.683 ns |         - |
| Serilog_OneString         | 105.395 ns |     144 B |
| Serilog_CodeGen_OneString | 302.269 ns |     776 B |
| Default_3Ints             |  35.648 ns |     120 B |
| Default_CodeGen_3Ints     |   1.687 ns |         - |
| Serilog_3Ints             | 215.985 ns |     544 B |
| Serilog_CodeGen_3Ints     | 597.040 ns |    1392 B |
