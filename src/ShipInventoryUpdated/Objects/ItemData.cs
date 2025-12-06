using Newtonsoft.Json;
using ShipInventoryUpdated.Dependencies.Newtonsoft;
using ShipInventoryUpdated.Helpers.API;
using Unity.Collections;
using Unity.Netcode;

namespace ShipInventoryUpdated.Objects;

/// <summary>
/// Data representing an item
/// </summary>
[Serializable]
public struct ItemData : INetworkSerializable, IEquatable<ItemData>
{
	/// <summary>
	/// Unique identifier for this item type
	/// </summary>
	[JsonConverter(typeof(FixedString32BytesJsonConverter))]
	public FixedString32Bytes ID;

	/// <summary>
	/// Scrap value of this item
	/// </summary>
	public int SCRAP_VALUE;

	/// <summary>
	/// Save data of this item
	/// </summary>
	public int SAVE_DATA;

	/// <summary>
	/// Has this item persisted through rounds or not
	/// </summary>
	public bool PERSISTED_THROUGH_ROUNDS;

	public ItemData() : this(null) { }

	public ItemData(GrabbableObject? item)
	{
		ID = ItemIdentifier.GetID(item?.itemProperties);
		SCRAP_VALUE = item?.scrapValue ?? 0;

		if (item?.itemProperties != null && item.itemProperties.saveItemVariable)
			SAVE_DATA = item.GetItemDataToSave();

		PERSISTED_THROUGH_ROUNDS = item?.scrapPersistedThroughRounds ?? false;
	}

	/// <summary>
	/// Fetches the item represented by this data
	/// </summary>
	public Item? GetItem() => ItemIdentifier.GetItem(ID.Value);

	/// <inheritdoc/>
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref ID);
		serializer.SerializeValue(ref SCRAP_VALUE);
		serializer.SerializeValue(ref SAVE_DATA);
		serializer.SerializeValue(ref PERSISTED_THROUGH_ROUNDS);
	}

	/// <inheritdoc/>
	public bool Equals(ItemData other)
	{
		if (ID != other.ID)
			return false;

		if (SCRAP_VALUE != other.SCRAP_VALUE)
			return false;

		if (SAVE_DATA != other.SAVE_DATA)
			return false;

		if (PERSISTED_THROUGH_ROUNDS != other.PERSISTED_THROUGH_ROUNDS)
			return false;

		return true;
	}
}