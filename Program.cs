using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace benchmark_serilog;

public class Program
{
    static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}

public static partial class Logging
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "{message}")]
    public static partial void Info(this ILogger logger, string message);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "{message} {n}")]
    public static partial void LogSomething(this ILogger logger, string message, int n);
}

[MemoryDiagnoser]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class NoSinkBenchmarks
{
    const string Message = "Keyboard not found. Press F1 to continue.";

    // Default ILogger (i.e. Microsoft.Extensions.Logging.ILogger).
    static readonly ILogger<NoSinkBenchmarks> Logger = LoggerFactory.Create(_ => { }).CreateLogger<NoSinkBenchmarks>();

    // SeriLog Logger.
    static readonly Logger SeriLogger = new LoggerConfiguration().CreateLogger();

    // SeriLog via default ILogger using compile-time logging source generation.
    static readonly ILogger<NoSinkBenchmarks> SeriDefaultLogger = new SerilogLoggerFactory().CreateLogger<NoSinkBenchmarks>();

    [Benchmark]
    public void Default() => Logger.LogInformation(Message);

    [Benchmark]
    public void Serilog() => SeriLogger.Information(Message);

    [Benchmark]
    public void Default_CodeGen_Something() => Logger.LogSomething(Message, 42);

    [Benchmark]
    public void Serilog_CodeGen_Something() => SeriDefaultLogger.LogSomething(Message, 42);

    [Benchmark]
    public void Default_CodeGen_Info() => Logger.Info(Message);

    [Benchmark]
    public void Serilog_CodeGen_Info() => SeriDefaultLogger.Info(Message);
}

[MemoryDiagnoser]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class ConsoleBenchmarks
{
    const string Message = "This statement is false.";

    // Default. Console logging.
    static readonly ILogger<ConsoleBenchmarks> Logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<ConsoleBenchmarks>();

    // SeriLog. Console logging.
    static readonly Logger SeriLogger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    // SeriLog. Console logging with Compile-time logging source generation.
    static readonly ILogger<ConsoleBenchmarks> SeriDefaultLogger = new SerilogLoggerFactory().CreateLogger<ConsoleBenchmarks>();

    // SeriLog Logger with async console sink.
    static readonly Logger SeriLoggerAsync = new LoggerConfiguration().WriteTo.Async(x => x.Console()).CreateLogger();

    // SeriLog Logger with async console sink.
    static readonly ILogger<ConsoleBenchmarks> SeriDefaultLoggerAsync = new SerilogLoggerFactory(new LoggerConfiguration().WriteTo.Async(x => x.Console()).CreateLogger()).CreateLogger<ConsoleBenchmarks>();

    [Benchmark]
    public void Default() => Logger.LogInformation(Message);

    [Benchmark]
    public void Serilog() => SeriLogger.Information(Message);

    [Benchmark]
    public void Serilog_Async() => SeriLoggerAsync.Information(Message);

    [Benchmark]
    public void Default_CodeGen_Something() => Logger.LogSomething(Message, 42);

    [Benchmark]
    public void Serilog_CodeGen_Something() => SeriDefaultLogger.LogSomething(Message, 42);

    [Benchmark]
    public void Serilog_CodeGen_Something_Async() => SeriDefaultLoggerAsync.LogSomething(Message, 42);

    [Benchmark]
    public void Default_CodeGen_Info() => Logger.Info(Message);

    [Benchmark]
    public void Serilog_CodeGen_Info() => SeriDefaultLogger.Info(Message);
}
