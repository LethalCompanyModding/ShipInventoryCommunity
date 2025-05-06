using HarmonyLib;
using UnityEngine;

namespace ShipInventoryUpdated.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRound_Patches
{
    /// <summary>
    /// Unlocks the chute when loading all the unlockables
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.LoadUnlockables))]
    private static void UnlockChute(StartOfRound __instance)
    {
        int index = -1;

        for (int i = 0; i < __instance.unlockablesList.unlockables.Count; i++)
        {
            if (__instance.unlockablesList.unlockables[i] == ShipInventoryUpdated.CHUTE_UNLOCK_ITEM)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Helpers.Logger.Error("Could not find the chute as an unlockable.");
            return;
        }

        __instance.UnlockShipObject(index);
    }

    /// <summary>
    /// Unlocks the chute back when the ship resets
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    private static void UnlockChuteBack(StartOfRound __instance)
    {
        if (!__instance.IsServer)
            return;
        
        UnlockChute(__instance);
    }
}