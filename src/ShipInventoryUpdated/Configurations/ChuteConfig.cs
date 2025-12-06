using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the chute itself
/// </summary>
internal class ChuteConfig
{
	private const string SECTION = "Chute";

	public readonly ConfigEntry<string> Blacklist;

	public ChuteConfig(ConfigFile cfg)
	{
		Blacklist = cfg.Bind(
			new ConfigDefinition(SECTION, "ChuteBlacklist"),
			"",
			new ConfigDescription(Localization.Get("configuration.chute.blacklist.description"))
		);
	}
}