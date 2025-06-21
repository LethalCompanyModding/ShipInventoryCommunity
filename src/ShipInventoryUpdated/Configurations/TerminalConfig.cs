using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the unlocking of the inventory
/// </summary>
internal class TerminalConfig
{
    private const string SECTION = "Terminal";
    
    public readonly ConfigEntry<string> InventoryCommand;

    public TerminalConfig(ConfigFile cfg)
    {
        InventoryCommand = cfg.Bind(
            new ConfigDefinition(SECTION, "InventoryCommand"),
            "ship",
            new ConfigDescription(Localization.Get("configuration.terminal.inventoryCommand.description"))
        );
    }
}