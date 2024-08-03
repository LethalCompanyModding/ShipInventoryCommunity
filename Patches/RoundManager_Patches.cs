using HarmonyLib;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManager_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    private static void DisableChute()
    {
        ChuteInteract.Instance?.gameObject.SetActive(false);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    private static void ClearShip(RoundManager __instance, bool despawnAllItems)
    {
        ChuteInteract.Instance?.gameObject.SetActive(true);
        
        // If one player is alive, skip
        if (!StartOfRound.Instance.allPlayersDead)
            return;
        
        // If the items didn't despawn, skip
        if (__instance.spawnedSyncedObjects.Count > 0)
            return;

        ChuteInteract.SetItems([]);
    }
}