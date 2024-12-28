using System.Linq;
using HarmonyLib;
using ShipInventory.Helpers;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    private static void ClearInventory(RoundManager __instance)
    {
        if (!__instance.IsServer)
            return;
        
        // Set all items to persist
        ItemManager.SetAllPersisted();
        
        // If players still alive, skip
        if (!StartOfRound.Instance.allPlayersDead)
            return;

        var items = ItemManager.GetItems().ToList();
        float keepRate = ShipInventory.Config.ActAsSafe.Value ? 1f : ShipInventory.Config.KeepRate.Value / 100f;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (UnityEngine.Random.value <= keepRate)
                items.RemoveAt(i);
        }
        
        // Clear the inventory
        ItemManager.RemoveItems(items.ToArray());
    }
}