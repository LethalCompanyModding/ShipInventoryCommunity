using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the inventory itself
/// </summary>
internal class PortableItemConfig
{
	private const string SECTION = "Portable Item";

	public readonly ConfigEntry<int> ItemCost;
	public readonly ConfigEntry<float> EnergyPerUse;

	public PortableItemConfig(ConfigFile cfg)
	{
		ItemCost = cfg.Bind(
			new ConfigDefinition(SECTION, "ItemCost"),
			5000,
			new ConfigDescription(Localization.Get("configuration.portableitem.itemCost.description"))
		);

		EnergyPerUse = cfg.Bind(
			new ConfigDefinition(SECTION, "EnergyPerUse"),
			10f,
			new ConfigDescription(Localization.Get("configuration.portableitem.energyPerUse.description"))
		);
	}
}