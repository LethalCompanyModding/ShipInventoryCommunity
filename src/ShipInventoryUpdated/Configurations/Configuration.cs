using BepInEx.Configuration;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds every other configuration
/// </summary>
internal class Configuration
{
    public readonly UnlockConfig Unlock;

    public Configuration(ConfigFile cfg)
    {
        Unlock = new UnlockConfig(cfg);
    }
}