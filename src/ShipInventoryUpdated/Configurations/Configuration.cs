using BepInEx.Configuration;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds every other configuration
/// </summary>
internal class Configuration
{
	public readonly ChuteConfig Chute;
	public readonly UnlockConfig Unlock;
	public readonly TerminalConfig Terminal;
	public readonly InventoryConfig Inventory;

	private Configuration(ConfigFile cfg)
	{
		Chute = new ChuteConfig(cfg);
		Unlock = new UnlockConfig(cfg);
		Terminal = new TerminalConfig(cfg);
		Inventory = new InventoryConfig(cfg);
	}

	/// <summary>
	/// Configuration loaded
	/// </summary>
	public static Configuration? Instance;

	/// <summary>
	/// Loads the configuration from the given configuration file
	/// </summary>
	public static void Load(ConfigFile cfg)
	{
		Instance = new Configuration(cfg);
	}
}