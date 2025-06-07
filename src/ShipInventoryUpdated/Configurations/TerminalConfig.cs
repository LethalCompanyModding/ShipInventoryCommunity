using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the terminal
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
            new ConfigDescription(Localization.Get("configuration.terminal.command.description"))
        );

        InventoryCommand.SettingChanged += (_, _) =>
        {
            Patches.Terminal_Patches.AssignNewCommand(InventoryCommand.Value);
        };
    }
}