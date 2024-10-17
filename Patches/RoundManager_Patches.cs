using System.Linq;
using HarmonyLib;
using ShipInventory.Helpers;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManager_Patches
{
    /// <summary>
    /// Clears the inventory
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    private static void ClearInventory(RoundManager __instance)
    {
        
        if (!__instance.IsServer)
            return;
        
        // Set all items to persist
        var items = ItemManager.GetItems().ToList();

        for (int i = 0; i < items.Count; i++)
        {
            var data = items[i];
            data.PERSISTED_THROUGH_ROUNDS = true;
            items[i] = data;
        }

        // Clear the inventory
        if (StartOfRound.Instance.allPlayersDead && !ShipInventory.Config.ActAsSafe.Value)
        {
            items.Clear();
            Logger.Debug("Clearing the ship...");
        }
        
        ItemManager.SetItems(items, true);
    }
}