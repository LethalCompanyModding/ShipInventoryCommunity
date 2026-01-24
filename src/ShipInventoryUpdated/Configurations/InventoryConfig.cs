using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the inventory itself
/// </summary>
internal class InventoryConfig : SyncedConfig2<InventoryConfig>
{
	private const string GUID_ = MyPluginInfo.PLUGIN_GUID + "." + nameof(InventoryConfig);
	private const string SECTION = "Inventory";

	[SyncedEntryField] public readonly SyncedEntry<int> MaxItemCount;
	[SyncedEntryField] public readonly SyncedEntry<bool> ClearOnWipe;
	[SyncedEntryField] public readonly SyncedEntry<float> RetrieveSpeed;

	public InventoryConfig(ConfigFile cfg) : base(GUID_)
	{
		MaxItemCount = cfg.BindSyncedEntry(
			new ConfigDefinition(SECTION, "MaxItemCount"),
			5_000,
			new ConfigDescription(Localization.Get("configuration.inventory.maxItemCount.description"))
		);
		
		ClearOnWipe = cfg.BindSyncedEntry(
			new ConfigDefinition(SECTION, "ClearOnWipe"),
			true,
			new ConfigDescription(Localization.Get("configuration.inventory.clearOnWipe.description"))
		);

		RetrieveSpeed = cfg.BindSyncedEntry(
			new ConfigDefinition(SECTION, "ChuteDelay"),
			0.5f,
			new ConfigDescription(Localization.Get("configuration.inventory.retrieveSpeed.description"))
		);
		
		ConfigManager.Register(this); 
	}
}