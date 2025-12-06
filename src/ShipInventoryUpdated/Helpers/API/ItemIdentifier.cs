using ShipInventoryUpdated.Dependencies.LethalLib;

namespace ShipInventoryUpdated.Helpers.API;

internal static class ItemIdentifier
{
	private const string INVALID_ITEM_ID = "InvalidItem";

	/// <summary>
	/// Fetches the generic ID of the given item
	/// </summary>
	public static string GetID(Item? item)
	{
		if (item == null)
			return INVALID_ITEM_ID;

		if (Dependency.Enabled)
		{
			var moddedID = Dependency.GetID(item);

			if (moddedID != null)
				return moddedID;
		}

		return $"Vanilla/{item.itemName}";
	}

	/// <summary>
	/// Fetches the item associated with the given ID
	/// </summary>
	public static Item? GetItem(string id)
	{
		if (id == INVALID_ITEM_ID)
			return null;

		if (Dependency.Enabled)
		{
			var moddedItem = Dependency.GetItem(id);

			if (moddedItem != null)
				return moddedItem;
		}

		foreach (var item in StartOfRound.Instance?.allItemsList?.itemsList ?? [])
		{
			var currentId = GetID(item);

			if (currentId == id)
				return item;
		}

		return null;
	}
}