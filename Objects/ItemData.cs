using System;
using ShipInventory.Helpers;
using Unity.Netcode;

namespace ShipInventory.Objects;

[Serializable]
public struct ItemData : INetworkSerializable
{
    public int ID;
    public int SCRAP_VALUE;
    public int SAVE_DATA;

    public ItemData()
    {
    }

    public Item? GetItem() => ItemManager.GetItem(this);

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref SCRAP_VALUE);
        serializer.SerializeValue(ref SAVE_DATA);
    }
}