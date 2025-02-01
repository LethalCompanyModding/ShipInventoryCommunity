using HarmonyLib;
using OpenMonitors.Monitors;
using ShipInventory.Items;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(LootMonitor))]
public class OpenMonitors_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootMonitor.Calculate))]
    private static void AddToLootMonitor(ref float __result)
    {
        __result += ItemManager.GetTotalValue(false, false);
    }
}