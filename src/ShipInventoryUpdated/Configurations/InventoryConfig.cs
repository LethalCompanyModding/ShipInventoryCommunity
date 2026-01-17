using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the inventory itself
/// </summary>
internal class InventoryConfig
{
	private const string SECTION = "Inventory";

	public readonly ConfigEntry<int> MaxItemCount;
	public readonly ConfigEntry<bool> ClearOnWipe;
	public readonly ConfigEntry<float> RetrieveSpeed;

	public InventoryConfig(ConfigFile cfg)
	{
		MaxItemCount = cfg.Bind(
			new ConfigDefinition(SECTION, "MaxItemCount"),
			5_000,
			new ConfigDescription(Localization.Get("configuration.inventory.maxItemCount.description"))
		);
		
		ClearOnWipe = cfg.Bind(
			new ConfigDefinition(SECTION, "ClearOnWipe"),
			true,
			new ConfigDescription(Localization.Get("configuration.inventory.clearOnWipe.description"))
		);

		RetrieveSpeed = cfg.Bind(
			new ConfigDefinition(SECTION, "ChuteDelay"),
			0.5f,
			new ConfigDescription(Localization.Get("configuration.inventory.retrieveSpeed.description"))
		);
	}
}