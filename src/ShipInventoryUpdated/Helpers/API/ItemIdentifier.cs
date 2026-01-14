using System.Security.Cryptography;
using System.Text;
using ShipInventoryUpdated.Dependencies.LethalLib;
using Unity.Collections;

namespace ShipInventoryUpdated.Helpers.API;

internal static class ItemIdentifier
{
	// TODO: Check if the INVALID_ITEM is something necessary
	private const string INVALID_ITEM_ID = "InvalidItem";
	private static readonly Dictionary<Item, string> ItemToHash = new();
	private static readonly Dictionary<string, Item> HashToItem = new();

	/// <summary>
	/// Fetches the hashed ID of the given item
	/// </summary>
	public static string GetID(Item? item)
	{
		if (item == null)
			return INVALID_ITEM_ID;

		if (ItemToHash.TryGetValue(item, out var hashedId))
			return hashedId;

		var id = GetGenericID(item);

		using var sha256Hash = SHA256.Create();

		var rawData = Encoding.Default.GetBytes(id);
		var hashedData = sha256Hash.ComputeHash(rawData);
		hashedId = Encoding.Default.GetString(hashedData);
		hashedId = new FixedString32Bytes(hashedId).ToString();

		ItemToHash.TryAdd(item, hashedId);
		HashToItem.TryAdd(hashedId, item);

		return hashedId;
	}

	/// <summary>
	/// Fetches the item associated with the given hashed ID
	/// </summary>
	public static Item? GetItem(string id)
	{
		if (id == INVALID_ITEM_ID)
			return null;

		if (HashToItem.TryGetValue(id, out var item))
			return item;

		item = GetItemFromHashedID(id);

		if (item == null)
			return null;

		HashToItem.TryAdd(id, item);
		ItemToHash.TryAdd(item, id);

		return item;
	}

	/// <summary>
	/// Fetches the generic ID of the given item
	/// </summary>
	private static string GetGenericID(Item item)
	{
		string? id = null;

		if (Dependency.Enabled)
			id = Dependency.GetID(item);

		return id ?? item.itemName;
	}

	/// <summary>
	/// Fetches the item associated with the given hashed ID
	/// </summary>
	private static Item? GetItemFromHashedID(string id)
	{
		Item? item = null;

		if (Dependency.Enabled)
			item = Dependency.GetItem(id);

		if (item == null)
		{
			var itemList = StartOfRound.Instance?.allItemsList?.itemsList ?? [];
			item = itemList.FirstOrDefault(i => GetID(i) == id);
		}

		return item;
	}
}