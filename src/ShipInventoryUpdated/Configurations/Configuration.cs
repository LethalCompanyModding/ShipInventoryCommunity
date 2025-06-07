using BepInEx.Configuration;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds every other configuration
/// </summary>
internal class Configuration
{
    public readonly TerminalConfig Terminal;

    public Configuration(ConfigFile cfg)
    {
        Terminal = new TerminalConfig(cfg);
    }
}