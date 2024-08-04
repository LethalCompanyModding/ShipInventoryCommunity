using HarmonyLib;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(GrabbableObject))]
public class GrabbableObject_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GrabbableObject.SetScrapValue))]
    private static bool PreventSubTextUpdate(GrabbableObject __instance, int setValueTo)
    {
        // Set value (in case of skip)
        __instance.scrapValue = setValueTo;

        // Skip if VentProp
        return __instance is not VentProp;
    }
}