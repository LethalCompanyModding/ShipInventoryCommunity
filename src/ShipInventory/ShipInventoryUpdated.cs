using BepInEx;
using HarmonyLib;

namespace ShipInventory;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class ShipInventoryUpdated : BaseUnityPlugin
{
    private void Awake()
    {
    }

    private Harmony? _harmony;
}
