using BepInEx.Logging;

namespace ShipInventory.Helpers;

/// <summary>
/// Helper to log things from this mod
/// </summary>
internal static class Logger
{
    private static ManualLogSource? log;
    
    public static void SetLogger(ManualLogSource logSource) => log = logSource;

    private static void Log(LogLevel level, object? content) => log?.Log(level, content ?? "null");
    public static void Info(object? content) => Log(LogLevel.Info, content);
    public static void Debug(object? content) => Log(LogLevel.Debug, content);
    public static void Error(object? content) => Log(LogLevel.Error, content);
}