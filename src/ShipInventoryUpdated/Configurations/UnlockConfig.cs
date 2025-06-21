using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the unlocking of the inventory
/// </summary>
internal class UnlockConfig
{
    private const string SECTION = "Unlock";
    
    public readonly ConfigEntry<string> UnlockName;
    public readonly ConfigEntry<int> UnlockCost;
    
    public UnlockConfig(ConfigFile cfg)
    {
        UnlockName = cfg.Bind(
            new ConfigDefinition(SECTION, "ChuteUnlockName"),
            "ship inventory",
            new ConfigDescription(Localization.Get("configuration.unlock.unlockName.description"))
        );
        
        UnlockCost = cfg.Bind(
            new ConfigDefinition(SECTION, "ChuteUnlockCost"),
            60,
            new ConfigDescription(Localization.Get("configuration.unlock.unlockCost.description"))
        );

        UnlockName.SettingChanged += (_, _) => Patches.Terminal_Patches.AssignNewCommand(UnlockName.Value);
        UnlockCost.SettingChanged += (_, _) => Patches.Terminal_Patches.AssignNewCost(UnlockCost.Value);
    }
}