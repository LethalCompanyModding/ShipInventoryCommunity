using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the mod itself
/// </summary>
internal class ModConfig
{
	private const string SECTION = "Mod";

	public readonly ConfigEntry<string> Language;

	public ModConfig(ConfigFile cfg)
	{
		Language = cfg.Bind(
			new ConfigDefinition(SECTION, "Language"),
			"en",
			new ConfigDescription(Localization.Get("configuration.mod.language.description"))
		);
	}
}