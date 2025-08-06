using System.Collections.Generic;
using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the unlocking of the inventory
/// </summary>
internal class TerminalConfig
{
    private const string SECTION = "Terminal";
    public enum SortOrder { NONE, NAME_ASC, NAME_DESC, VALUE_ASC, VALUE_DESC }
    
    public readonly ConfigEntry<string> InventoryCommand;
    public readonly ConfigEntry<SortOrder> InventorySortOrder;
    public readonly ConfigEntry<bool> AutomaticPositiveAnswer;

    public TerminalConfig(ConfigFile cfg)
    {
        InventoryCommand = cfg.Bind(
            new ConfigDefinition(SECTION, "InventoryCommand"),
            "ship",
            new ConfigDescription(Localization.Get("configuration.terminal.command.description"))
        );
        
        InventorySortOrder = cfg.Bind(
            new ConfigDefinition(SECTION, "InventorySortOrder"),
            SortOrder.NAME_ASC,
            new ConfigDescription(Localization.Get("configuration.terminal.inventorySortOrder.description"))
        );
        
        AutomaticPositiveAnswer = cfg.Bind(
            new ConfigDefinition(SECTION, "YesPlease"),
            false,
            new ConfigDescription(Localization.Get("configuration.terminal.automaticPositive.description", new Dictionary<string, string>
            {
                ["positiveAnswer"] = Localization.Get("application.answers.positive")
            }))
        );
    }
}