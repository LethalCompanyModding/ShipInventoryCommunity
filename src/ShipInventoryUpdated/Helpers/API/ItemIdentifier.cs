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

		return $"Vanilla/{item.itemName}";
	}

	/// <summary>
	/// Fetches the generic ID of the given item
	/// </summary>
	public static Item? GetItem(string id)
	{
		if (id == INVALID_ITEM_ID)
			return null;

		foreach (var item in StartOfRound.Instance?.allItemsList?.itemsList ?? [])
		{
			var currentId = GetID(item);

			if (currentId == id)
				return item;
		}

		return null;
	}
}