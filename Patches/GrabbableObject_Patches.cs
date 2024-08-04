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
        __instance.scrapValue = setValueTo;

        return __instance is not VentProp;
    }
}