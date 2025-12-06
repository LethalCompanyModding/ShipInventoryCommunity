using ShipInventoryUpdated.Objects;

namespace ShipInventoryUpdated.Helpers.API;

/// <summary>
/// Helper to handle the conversion of items into <see cref="ItemData"/>
/// </summary>
public static class ItemConverter
{
	#region API

	private static readonly List<Func<GrabbableObject, ItemData[]?>> conversions = [];

	/// <summary>
	/// Adds a conversion that defines how the given object is converted to its data form
	/// </summary>
	/// <param name="conversion">Returns the list of data for the given object or <c>null</c> if this conversion doesn't handle this object</param>
	public static void AddConversion(Func<GrabbableObject, ItemData[]?> conversion) => conversions.Add(conversion);

	/// <summary>
	/// Converts the given item into <see cref="ItemData"/>, using registered conversions
	/// </summary>
	internal static ItemData[] Convert(GrabbableObject item)
	{
		foreach (var conversion in conversions)
		{
			var items = conversion?.Invoke(item);

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

	private static ItemData[] NormalConversion(GrabbableObject item) => [new(item)];

	private static ItemData[]? BeltBagConversion(GrabbableObject item)
	{
		if (item is not BeltBagItem beltBagItem)
			return null;

		var items = new List<ItemData>();

		for (var i = beltBagItem.objectsInBag.Count - 1; i >= 0; i--)
		{
			var _item = beltBagItem.objectsInBag[i];

			if (_item == null)
				continue;

			items.AddRange(Convert(_item));
		}

		items.Add(new ItemData(beltBagItem));

		return items.ToArray();
	}

	#endregion
}