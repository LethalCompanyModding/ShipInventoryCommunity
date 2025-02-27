using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using OpenMonitors.Monitors;
using ShipInventory.Patches;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Compatibility;

// https://github.com/julian-perge/LC_OpenMonitors/tree/main/OpenMonitors
internal static class OpenMonitors
{
    public const string GUID = "xxxstoner420bongmasterxxx.open_monitors";
    private static bool? _enabled;

    public static bool Enabled
    {
        get
        {
            _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);
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
            Logger.Error($"Failed to apply OpenMonitors patches. {ex}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void UpdateMonitor() => LootMonitor.Instance.UpdateMonitor();
}
