using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using ShipInventory.Helpers;

namespace ShipInventory.Objects;

public class ShipInventoryConfigs
{
    public readonly ConfigEntry<string> blacklist;
    public readonly ConfigEntry<float> spawnDelay;

    public ShipInventoryConfigs(ConfigFile cfg)
    {
        cfg.SaveOnConfigSet = false;

        blacklist = cfg.Bind(
            "General",
            "ChuteBlacklist",
            "",
            "List of items that are not allowed in the chute.\nThe items' name should be separated by a comma (,)."
        );
        blacklist.SettingChanged += (_, _) => ItemManager.UpdateBlacklist();

        spawnDelay = cfg.Bind(
            "General",
            "ChuteDelay",
            0.5f,
            "Time in seconds between each item spawn."
        );

        ClearOrphanedEntries(cfg);
        cfg.Save();
        cfg.SaveOnConfigSet = true;
    }

    private static void ClearOrphanedEntries(ConfigFile cfg)
    {
        // Find the private property `OrphanedEntries` from the type `ConfigFile`
        PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");

        // And get the value of that property from our ConfigFile instance
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);

        // And finally, clear the `OrphanedEntries` dictionary
        orphanedEntries.Clear();
    }
}