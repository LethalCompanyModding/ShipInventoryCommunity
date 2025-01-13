using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using OpenMonitors.Monitors;
using ShipInventory.Patches;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Compatibility;

public static class OpenMonitorsCompatibility
{
    public const string OPEN_MONITOR = "xxxstoner420bongmasterxxx.open_monitors";
    private static bool? _enabled;

    public static bool Enabled
    {
        get
        {
            _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(OPEN_MONITOR);
            return _enabled.Value;
        }
    }
    
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void PatchAll(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(OpenMonitors_Patches));
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to apply ShipInventory patches. {ex}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void UpdateMonitor()
    {
        LootMonitor.Instance?.UpdateMonitor();
    }
}
