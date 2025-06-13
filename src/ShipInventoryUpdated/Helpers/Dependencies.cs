using System.Collections.Generic;
using ShipInventoryUpdated.Configurations;

namespace ShipInventoryUpdated.Helpers;

/// <summary>
/// Helper to manage dependencies easier
/// </summary>
internal static class Dependencies
{
    private static readonly Dictionary<string, bool> cachedStatus = [];

    /// <summary>
    /// Checks if the plugin with the given GUID has been loaded
    /// </summary>
    public static bool IsEnabled(string guid)
    {
        if (cachedStatus.TryGetValue(guid, out var isEnabled))
            return isEnabled;
        
        isEnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(guid);
        cachedStatus.Add(guid, isEnabled);
        return isEnabled;
    }

    /// <summary>
    /// Loads all the dependencies
    /// </summary>
    public static void LoadDependencies(Configuration? config)
    {
        if (global::ShipInventoryUpdated.Dependencies.LethalConfig.Dependency.Enabled)
            global::ShipInventoryUpdated.Dependencies.LethalConfig.Dependency.ApplyConfiguration(config);
    }
}