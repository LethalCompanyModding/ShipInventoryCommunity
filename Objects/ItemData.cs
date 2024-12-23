using System;
using ShipInventory.Compatibility;
using ShipInventory.Helpers;
using Unity.Netcode;

namespace ShipInventory.Objects;

/// <summary>
/// Data transferred between the clients and the host
/// </summary>
[Serializable]
public struct ItemData : INetworkSerializable
{
    #nullable disable
    public static Item FALLBACK_ITEM; // Item to use when the original item is missing
    #nullable restore
    
    public string ID;
    
    public int SCRAP_VALUE;
    public int SAVE_DATA;
    public bool PERSISTED_THROUGH_ROUNDS;
    
    public ItemData(GrabbableObject item)
    {
        ID = LethalLibCompatibility.GetID(item.itemProperties);
        Logger.Info(ID);
        
        if (item.itemProperties.isScrap)
            SCRAP_VALUE = item.scrapValue;
    
        if (item.itemProperties.saveItemVariable)
            SAVE_DATA = item.GetItemDataToSave();

        PERSISTED_THROUGH_ROUNDS = item.scrapPersistedThroughRounds;
    }

    public Item GetItem() => LethalLibCompatibility.GetItem(ID) ?? FALLBACK_ITEM;
    public string GetItemName() => GetItem().itemName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref SCRAP_VALUE);
        serializer.SerializeValue(ref SAVE_DATA);
        serializer.SerializeValue(ref PERSISTED_THROUGH_ROUNDS);
    }
}