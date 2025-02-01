using System;
using ShipInventory.Compatibility;
using Unity.Netcode;

namespace ShipInventory.Items;

/// <summary>
/// Data transferred between the clients and the host
/// </summary>
[Serializable]
public class ItemData : INetworkSerializable
{
    public string ID;
    public int SCRAP_VALUE;
    public int SAVE_DATA;
    public bool PERSISTED_THROUGH_ROUNDS;
    
    public ItemData(GrabbableObject item)
    {
        ID = Compatibility.LethalLib.GetID(item.itemProperties);
        
        if (item.itemProperties.isScrap)
            SCRAP_VALUE = item.scrapValue;
    
        if (item.itemProperties.saveItemVariable)
            SAVE_DATA = item.GetItemDataToSave();

        PERSISTED_THROUGH_ROUNDS = item.scrapPersistedThroughRounds;
    }

    public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref SCRAP_VALUE);
        serializer.SerializeValue(ref SAVE_DATA);
        serializer.SerializeValue(ref PERSISTED_THROUGH_ROUNDS);
    }
}