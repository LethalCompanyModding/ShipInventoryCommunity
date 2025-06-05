using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ShipInventoryUpdated;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class ShipInventoryUpdated : BaseUnityPlugin
{
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);

        if (!LoadAssets("si-bundle"))
            return;
        
        Patch();
        
        Helpers.Logger.Info($"{LCMPluginInfo.PLUGIN_GUID} v{LCMPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    #region Bundle

    internal static GameObject? CHUTE_PREFAB;

    private static bool LoadAssets(string name)
    {
        if (!Helpers.Bundle.LoadBundle(name))
            return false;

        CHUTE_PREFAB = Helpers.Bundle.LoadAsset<GameObject>("ChutePrefab");

        return true;
    }

    #endregion

    #region Patches

    private Harmony? Harmony;

    private void Patch()
    {
        Harmony = new Harmony(LCMPluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(typeof(Patches.GameNetworkManager_Patches));
    }

    #endregion
}
