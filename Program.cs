using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace benchmark_serilog;

public class Point
{
    public int X { get; init; }
    public int Y { get; init; }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public class Program
{
    public const string Message = "Keyboard not found. Press F1 to continue.";
    public static readonly Point Point = new() { X = 1, Y = 2 };

    static void Main(string[] args)
        => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}

[MemoryDiagnoser]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class NoSinkBenchmarks
{
    // Default ILogger (i.e. Microsoft.Extensions.Logging.ILogger).
    static readonly ILogger<NoSinkBenchmarks> Logger =
        LoggerFactory.Create(_ => { }).CreateLogger<NoSinkBenchmarks>();

    // SeriLog Logger.
    static readonly Logger SeriLogger = new LoggerConfiguration().CreateLogger();

    // SeriLog via default ILogger using compile-time logging source generation.
    static readonly ILogger<NoSinkBenchmarks> SeriDefaultLogger =
        new SerilogLoggerFactory().CreateLogger<NoSinkBenchmarks>();

    [Benchmark]
    public void Default() => Logger.LogInformation(Program.Message);

    [Benchmark]
    public void Serilog() => SeriLogger.Information(Program.Message);

    [Benchmark]
    public void Default_CodeGen_Something() => Logger.LogSomething(Program.Message, 42);

    [Benchmark]
    public void Serilog_CodeGen_Something() => SeriDefaultLogger.LogSomething(Program.Message, 42);

    [Benchmark]
    public void Default_CodeGen_Info() => Logger.Info(Program.Message);

    [Benchmark]
    public void Serilog_CodeGen_Info() => SeriDefaultLogger.Info(Program.Message);

    [Benchmark]
    public void Serilog_CodeGen_Point() => SeriDefaultLogger.LogPoint(Program.Message, Program.Point);
}

[MemoryDiagnoser]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class ConsoleBenchmarks
{
    // Default. Console logging.
    static readonly ILogger<ConsoleBenchmarks> Logger =
        LoggerFactory.Create(b => b.AddConsole()).CreateLogger<ConsoleBenchmarks>();

    // SeriLog. Console logging.
    static readonly Logger SeriLogger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    // SeriLog. Console logging with Compile-time logging source generation.
    static readonly ILogger<ConsoleBenchmarks> SeriDefaultLogger =
        new SerilogLoggerFactory(
            new LoggerConfiguration().WriteTo.Console().CreateLogger())
            .CreateLogger<ConsoleBenchmarks>();

    // SeriLog Logger with async console sink.
    static readonly Logger SeriLoggerAsync =
        new LoggerConfiguration().WriteTo.Async(x => x.Console()).CreateLogger();

    // SeriLog Logger with async console sink.
    static readonly ILogger<ConsoleBenchmarks> SeriDefaultLoggerAsync =
        new SerilogLoggerFactory(
            new LoggerConfiguration().WriteTo.Async(x => x.Console()).CreateLogger())
            .CreateLogger<ConsoleBenchmarks>();

    [Benchmark]
    public void Default() => Logger.LogInformation(Program.Message);

    [Benchmark]
    public void Default_CodeGen_Info() => Logger.Info(Program.Message);

    [Benchmark]
    public void Default_CodeGen_Point() => Logger.LogPoint(Program.Message, Program.Point);

    [Benchmark]
    public void Default_CodeGen_Something() => Logger.LogSomething(Program.Message, 42);

    [Benchmark]
    public void Serilog() => SeriLogger.Information(Program.Message);

    [Benchmark]
    public void Serilog_Async() => SeriLoggerAsync.Information(Program.Message);

    [Benchmark]
    public void Serilog_CodeGen_Info() => SeriDefaultLogger.Info(Program.Message);

    [Benchmark]
    public void Serilog_CodeGen_Info_Async() => SeriDefaultLoggerAsync.Info(Program.Message);

    [Benchmark]
    public void Serilog_CodeGen_Something() => SeriDefaultLogger.LogSomething(Program.Message, 42);

    [Benchmark]
    public void Serilog_CodeGen_Something_Async() => SeriDefaultLoggerAsync.LogSomething(Program.Message, 42);

    [Benchmark]
    public void Serilog_Point() => SeriLogger.Information("{Message} {@Point}", Program.Message, Program.Point);

    [Benchmark]
    public void Serilog_Point_Async() => SeriDefaultLoggerAsync.LogInformation("{Message} {@Point}", Program.Message, Program.Point);
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

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "{message} {point}")]
    public static partial void LogPoint(this ILogger logger, string message, Point point);
}
