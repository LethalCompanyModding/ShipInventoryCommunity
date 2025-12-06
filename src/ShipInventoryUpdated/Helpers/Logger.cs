using BepInEx.Logging;

namespace ShipInventoryUpdated.Helpers;

/// <summary>
/// Helper to log things from this mod
/// </summary>
internal static class Logger
{
	private static ManualLogSource? log;

	public static void SetLogger(ManualLogSource logSource) => log = logSource;

	private static void Log(LogLevel level, object? content) => log?.Log(level, content ?? "null");

	/// <summary>
	/// Logs information for developers to know important steps of the mod
	/// </summary>
	public static void Trace(object? content) => Log(LogLevel.Message, content);

	/// <summary>
	/// Logs information for developers that helps to debug the mod
	/// </summary>
	public static void Debug(object? content) => Log(LogLevel.Debug, content);

	/// <summary>
	/// Logs information for players to know important steps of the mod
	/// </summary>
	public static void Info(object? content) => Log(LogLevel.Info, content);

	/// <summary>
	/// Logs information for players to warn them about an unwanted state
	/// </summary>
	public static void Warn(object? content) => Log(LogLevel.Warning, content);

	/// <summary>
	/// Logs information for players to notify them of an error
	/// </summary>
	public static void Error(object? content) => Log(LogLevel.Error, content);
}