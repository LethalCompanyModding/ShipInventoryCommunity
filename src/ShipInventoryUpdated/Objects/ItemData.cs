using System;
using Unity.Collections;
using Unity.Netcode;

namespace ShipInventoryUpdated.Objects;

/// <summary>
/// Data representing an item
/// </summary>
[Serializable]
public struct ItemData : INetworkSerializable, IEquatable<ItemData>
{
    public FixedString32Bytes ID;
    public int SCRAP_VALUE;

    public ItemData() : this(null) { }

    public ItemData(GrabbableObject? item)
    {
        ID = item?.itemProperties?.itemName ?? "";
        SCRAP_VALUE = item?.scrapValue ?? 0;
    }
    
    /// <inheritdoc/>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref SCRAP_VALUE);
    }

    /// <inheritdoc/>
    public bool Equals(ItemData other)
    {
        if (ID != other.ID)
            return false;

        if (SCRAP_VALUE != other.SCRAP_VALUE)
            return false;

        return true;
    }
}