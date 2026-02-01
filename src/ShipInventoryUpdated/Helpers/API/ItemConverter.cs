using ShipInventoryUpdated.Objects;

namespace ShipInventoryUpdated.Helpers.API;

/// <summary>
/// Helper to handle the conversion of items into <see cref="ItemData"/>
/// </summary>
public static class ItemConverter
{
	#region API

	private static readonly List<Func<GrabbableObject, bool, ItemData[]?>> Conversions = [];

	/// <summary>
	/// Adds a conversion that defines how the given object is converted to its data form
	/// </summary>
	/// <param name="conversion">Returns the list of data for the given object or <c>null</c> if this conversion doesn't handle this object</param>
	public static void AddConversion(Func<GrabbableObject, bool, ItemData[]?> conversion) => Conversions.Add(conversion);

	/// <summary>
	/// Converts the given item into <see cref="ItemData"/>, using registered conversions
	/// </summary>
	internal static ItemData[] Convert(GrabbableObject item, bool addSaveData)
	{
		foreach (var conversion in Conversions)
		{
			var items = conversion?.Invoke(item, addSaveData);

			if (items != null)
				return items;
		}

		return [];
	}

	#endregion

	#region Conversions

	internal static void LoadConversions()
	{
		AddConversion(BeltBagConversion);
		AddConversion(NormalConversion);
	}

	private static ItemData[] NormalConversion(GrabbableObject item, bool addSaveData) => [new(item, addSaveData)];

	private static ItemData[]? BeltBagConversion(GrabbableObject item, bool addSaveData)
	{
		if (item is not BeltBagItem beltBagItem)
			return null;

		var items = new List<ItemData>();

		for (var i = beltBagItem.objectsInBag.Count - 1; i >= 0; i--)
		{
			var itemInBag = beltBagItem.objectsInBag[i];

			if (itemInBag == null)
				continue;

			items.AddRange(Convert(itemInBag, addSaveData));
		}

		items.AddRange(NormalConversion(beltBagItem, addSaveData));

		return items.ToArray();
	}

	#endregion
}