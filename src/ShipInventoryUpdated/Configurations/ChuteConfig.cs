using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the chute itself
/// </summary>
internal class ChuteConfig : SyncedConfig2<ChuteConfig>
{
	private const string GUID_ = MyPluginInfo.PLUGIN_GUID + "." + nameof(ChuteConfig);
	private const string SECTION = "Chute";

	public readonly ConfigEntry<string> Blacklist;
	public readonly ConfigEntry<float> StoreSpeed;

	public ChuteConfig(ConfigFile cfg) : base(GUID_)
	{
		Blacklist = cfg.Bind(
			new ConfigDefinition(SECTION, "ChuteBlacklist"),
			"",
			new ConfigDescription(Localization.Get("configuration.chute.blacklist.description"))
		);

		StoreSpeed = cfg.Bind(
			new ConfigDefinition(SECTION, "TimeToStore"),
			0.5f,
			new ConfigDescription(Localization.Get("configuration.chute.storeSpeed.description"))
		);
	}
}