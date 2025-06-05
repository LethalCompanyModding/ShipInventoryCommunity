using BepInEx;

namespace ShipInventoryUpdated;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class ShipInventoryUpdated : BaseUnityPlugin
{
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);
        
        Helpers.Logger.Info($"{LCMPluginInfo.PLUGIN_GUID} v{LCMPluginInfo.PLUGIN_VERSION} has loaded!");
    }
}
