using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Serilog.Sinks.OpenTelemetry;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace benchmark_serilog;

public static partial class Logging
{
	// Default. Console logging.
	public static readonly ILogger<Program> Logger =
		LoggerFactory.Create(b => b.AddConsole()).CreateLogger<Program>();

	// SeriLog. Console logging.
	public static readonly Logger SeriLogger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

	// SeriLog Logger with async console sink.
	public static readonly Logger SeriLoggerAsync =
		new LoggerConfiguration().WriteTo.Async(x => x.Console()).CreateLogger();

	// SeriLog Logger with async console sink.
	public static readonly ILogger<Program> SeriDefaultLoggerAsync =
		new SerilogLoggerFactory(
			new LoggerConfiguration().WriteTo.Async(x => x.Console()).CreateLogger())
			.CreateLogger<Program>();

	// SeriLog. Open Telemetry logging.
	public static readonly Logger SeriSigNozLogger = new LoggerConfiguration()
		.WriteTo.OpenTelemetry(options =>
		{
			options.Endpoint = "http://127.0.0.1:4317";
			options.Protocol = OtlpProtocol.Grpc;
			options.ResourceAttributes = new Dictionary<string, object> { ["service.name"] = "benchmark-SeriSigNozLogger" };
		}).CreateLogger();

	public static readonly ILogger<Program> SeriSigNozILogger =
		new SerilogLoggerFactory(
			new LoggerConfiguration()
				.WriteTo.OpenTelemetry(options =>
				{
					options.Endpoint = "http://127.0.0.1:4317";
					options.Protocol = OtlpProtocol.Grpc;
					options.ResourceAttributes = new Dictionary<string, object> { ["service.name"] = "benchmark-SeriSigNozILogger" };
				}).CreateLogger())
			.CreateLogger<Program>();

	public static readonly ILogger DefaultSigNozLogger = LoggerFactory.Create(builder =>
	{
		builder.AddOpenTelemetry(options =>
		{
			options.IncludeScopes = false;
			options.ParseStateValues = true;
			options.IncludeFormattedMessage = true;
			options.AddOtlpExporter(otlpOptions =>
			{
				otlpOptions.Endpoint = new Uri("http://localhost:4317");
				otlpOptions.Protocol = OtlpExportProtocol.Grpc;
			});
		});
	}).CreateLogger<Program>();

	[LoggerMessage(
		EventId = 0,
		Level = LogLevel.Information,
		Message = "{message}")]
	public static partial void Info(this ILogger logger, string message);

	[LoggerMessage(
		EventId = 1,
		Level = LogLevel.Information,
		Message = "{message} {point}")]
	public static partial void LogPoint(this ILogger logger, string message, Point point);

	[LoggerMessage(
		EventId = 2,
		Level = LogLevel.Information,
		Message = "{message} {x} {y} {z}")]
	public static partial void LogInts(this ILogger logger, string message, int x, int y, int z);
}

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

/// <summary>
/// Comparing default Microsoft Logger vs Serilog vs Serilog async console.
/// </summary>
[HideColumns("Error", "StdDev", "Median")]
[MemoryDiagnoser(displayGenColumns: false)]
public class LogInfoOnConsole
{
	[Benchmark]
	public void Default() => Logging.Logger.LogInformation(Program.Message);

	[Benchmark]
	public void Serilog() => Logging.SeriLogger.Information(Program.Message);

	[Benchmark]
	public void SerilogAsync() => Logging.SeriLoggerAsync.Information(Program.Message);
}

/// <summary>
/// Async console results indicate hugh speed up by using Serilog using background thread to log.
/// Benchmarks from now on will only use async.
///
/// Using Serilog async console sink, comparing source generated Info vs `Information` method on Serilog's interface.
/// </summary>
[HideColumns("Error", "StdDev", "Median")]
[MemoryDiagnoser(displayGenColumns: false)]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class SerilogAsyncConsoleInfo
{
	[Benchmark]
	public void CodeGenInfo() => Logging.SeriDefaultLoggerAsync.Info(Program.Message);

	[Benchmark]
	public void Info() => Logging.SeriLoggerAsync.Information(Program.Message);
}

/// <summary>
/// Comparing getting a Point containing X / Y ints into the body of the message and into the
/// structured log.
/// </summary>
[HideColumns("Error", "StdDev", "Median")]
[MemoryDiagnoser(displayGenColumns: false)]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class SerilogAsyncConsolePoint
{
	[Benchmark]
	public void CodeGenPoint() => Logging.SeriDefaultLoggerAsync.LogPoint("Processed", Program.Point);

	[Benchmark]
	public void Point() => Logging.SeriLoggerAsync.Information("Processed {@Point}", Program.Point);
}

/// <summary>
/// Comparing getting 3 ints into the body of the message and into the structured log.
/// </summary>
[HideColumns("Error", "StdDev", "Median")]
[MemoryDiagnoser(displayGenColumns: false)]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class SerilogAsyncConsoleInts
{
	[Benchmark]
	public void CodeGenInts() => Logging.SeriDefaultLoggerAsync.LogInts("Processed", 10, 20, 30);

	[Benchmark]
	public void Ints() => Logging.SeriLoggerAsync.Information("Processed {@0} {@1} {@2}", 10, 20, 30);
}

/// <summary>
/// Comparing Serilog async console sink to local SigNoz vs Serilog async to local SigNoz
/// </summary>
[HideColumns("Error", "StdDev", "Median")]
[MemoryDiagnoser(displayGenColumns: false)]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class SerilogAsyncConsoleVsSerilogToSigNoz
{
	[Benchmark]
	public void AsyncSigNozInfo() => Logging.SeriSigNozLogger.Information(Program.Message);

	[Benchmark]
	public void AsyncInfo() => Logging.SeriLoggerAsync.Information(Program.Message);
}

/// <summary>
/// Comparing Microsoft default logger to local SigNoz vs Serilog async to local SigNoz
/// </summary>
[HideColumns("Error", "StdDev", "Median")]
[MemoryDiagnoser(displayGenColumns: false)]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class MicrosoftLoggerVsSerilogToSigNoz
{
	[Benchmark]
	public void Default_OneString() => Logging.DefaultSigNozLogger.LogInformation(Program.Message);

	[Benchmark]
	public void Default_CodeGen_OneString() => Logging.DefaultSigNozLogger.Info(Program.Message);

	[Benchmark]
	public void Serilog_OneString() => Logging.SeriSigNozLogger.Information(Program.Message);

	[Benchmark]
	public void Serilog_CodeGen_OneString() => Logging.SeriSigNozILogger.Info(Program.Message);

	[Benchmark]
	public void Default_3Ints() => Logging.DefaultSigNozLogger.LogInformation("Test {} {} {}", 1, 2, 3);

	[Benchmark]
	public void Default_CodeGen_3Ints() => Logging.DefaultSigNozLogger.LogInts("Test ", 1, 2, 3);

	[Benchmark]
	public void Serilog_3Ints() => Logging.SeriSigNozLogger.Information("Test {@0} {@1} {@2}", 1, 2, 3);

	[Benchmark]
	public void Serilog_CodeGen_3Ints() => Logging.SeriSigNozILogger.LogInts("Test ", 1, 2, 3);
}

/// <summary>
/// Using no sink, compare Microsoft default logger to serilog.
/// </summary>
[HideColumns("Error", "StdDev", "Median")]
[MemoryDiagnoser(displayGenColumns: false)]
[SuppressMessage("Performance", "CA1822:Mark members as static")] // benchmarks must be non-static
public class NoSinkLoggerVsSerilog
{
	// Default. No sink.
	static readonly ILogger<Program> Logger =
		LoggerFactory.Create(_ => { }).CreateLogger<Program>();

	// SeriLog. No sink.
	static readonly Logger SeriLogger = new LoggerConfiguration().CreateLogger();

	// SeriLog via ILogger. No sink.
	static readonly ILogger<Program> SeriILogger =
		new SerilogLoggerFactory(
			new LoggerConfiguration().CreateLogger())
			.CreateLogger<Program>();

	[Benchmark]
	public void Default_OneString() => Logger.LogInformation(Program.Message);

	[Benchmark]
	public void Default_CodeGen_OneString() => Logger.Info(Program.Message);

	[Benchmark]
	public void Serilog_OneString() => SeriLogger.Information(Program.Message);

	[Benchmark]
	public void Serilog_CodeGen_OneString() => SeriILogger.Info(Program.Message);

	[Benchmark]
	public void Default_3Ints() => Logger.LogInformation("Test {} {} {}", 1, 2, 3);

	[Benchmark]
	public void Default_CodeGen_3Ints() => Logger.LogInts("Test ", 1, 2, 3);

	[Benchmark]
	public void Serilog_3Ints() => SeriLogger.Information("Test {@0} {@1} {@2}", 1, 2, 3);

	[Benchmark]
	public void Serilog_CodeGen_3Ints() => SeriILogger.LogInts("Test ", 1, 2, 3);
}
