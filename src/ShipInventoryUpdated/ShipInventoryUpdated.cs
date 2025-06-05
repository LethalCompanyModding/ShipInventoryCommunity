using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ShipInventoryUpdated;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
// Hard
// Soft
[BepInDependency(LethalConfig.PluginInfo.Guid, BepInDependency.DependencyFlags.SoftDependency)]
public class ShipInventoryUpdated : BaseUnityPlugin
{
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);

        if (!LoadAssets("si-bundle"))
            return;
        
        LoadDependencies();
        Patch();
        
        Helpers.Logger.Info($"{LCMPluginInfo.PLUGIN_GUID} v{LCMPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    #region Bundle

    internal static GameObject? CHUTE_PREFAB;
    internal static TerminalNode? CHUTE_BUY_NODE;
    internal static TerminalNode? CHUTE_CONFIRM_NODE;

    private static bool LoadAssets(string name)
    {
        if (!Helpers.Bundle.LoadBundle(name))
            return false;

        CHUTE_PREFAB = Helpers.Bundle.LoadAsset<GameObject>("ChutePrefab");
        CHUTE_BUY_NODE = Helpers.Bundle.LoadAsset<TerminalNode>("ChuteBuy");
        CHUTE_CONFIRM_NODE = Helpers.Bundle.LoadAsset<TerminalNode>("ChuteConfirm");

        return true;
    }

    #endregion
    
    #region Dependencies

    private static void LoadDependencies()
    {
        if (Dependencies.LethalConfig.Dependency.Enabled)
            Dependencies.LethalConfig.Dependency.ApplyConfiguration();
    }

    #endregion

    #region Patches

    private Harmony? Harmony;

    private void Patch()
    {
        Harmony = new Harmony(LCMPluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(typeof(Patches.GameNetworkManager_Patches));
        Harmony.PatchAll(typeof(Patches.Terminal_Patches));
    }

    #endregion
}
