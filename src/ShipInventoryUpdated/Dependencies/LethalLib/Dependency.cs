using System.Runtime.CompilerServices;
using ShipInventoryUpdated.Helpers.API;

namespace ShipInventoryUpdated.Dependencies.LethalLib;

// https://github.com/EvaisaDev/LethalLib
internal abstract class Dependency
{
	public static bool Enabled => Helpers.Dependencies.IsEnabled(global::LethalLib.Plugin.ModGUID);

	private static Dictionary<string, Item>? _cachedModdedItems;

	private static void LoadModdedItems()
	{
		// If already loaded, skip
		if (_cachedModdedItems != null)
			return;

		_cachedModdedItems = [];

		foreach (var item in global::LethalLib.Modules.Items.scrapItems)
			_cachedModdedItems.TryAdd($"{item.modName}/{item.item.itemName}", item.item);

		foreach (var item in global::LethalLib.Modules.Items.shopItems)
			_cachedModdedItems.TryAdd($"{item.modName}/{item.item.itemName}", item.item);

		foreach (var item in global::LethalLib.Modules.Items.plainItems)
			_cachedModdedItems.TryAdd($"{item.modName}/{item.item.itemName}", item.item);
	}

	/// <summary>
	/// Fetches the generic ID of the given modded item
	/// </summary>
	public static string? GetID(Item item)
	{
		LoadModdedItems();

		if (_cachedModdedItems == null)
			return null;

		foreach ((var id, var moddedItem) in _cachedModdedItems)
		{
			if (moddedItem == item)
				return id;
		}

		return null;
	}

	/// <summary>
	/// Fetches the modded item associated with the given hashed ID
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static Item? GetItem(string id)
	{
		LoadModdedItems();

		if (_cachedModdedItems == null)
			return null;

		foreach ((_, var moddedItem) in _cachedModdedItems)
		{
			var hashedId = ItemIdentifier.GetID(moddedItem);
			
			if (hashedId == id)
				return moddedItem;
		}

		return null;
	}
}