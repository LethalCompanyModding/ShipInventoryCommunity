using System;
using ShipInventory.Commands;
using Unity.Netcode;
using UnityEngine;

namespace ShipInventory.Objects;

[Serializable]
public struct ItemData : INetworkSerializable
{
    public int ID = 0;
    public int SCRAP_VALUE = 0;
    public int SAVE_DATA = 0;

    public ItemData()
    {
    }

    public static ItemData Save(GrabbableObject item)
    {
        ItemData data = default;

        data.ID = StartOfRound.Instance.allItemsList.itemsList.FindIndex(i => i == item.itemProperties);
        
        if (item.itemProperties.isScrap)
            data.SCRAP_VALUE = item.scrapValue;
        
        if (item.itemProperties.saveItemVariable)
            data.SAVE_DATA = item.GetItemDataToSave();
        
        return data;
    }

    public Item? GetItem() => StartOfRound.Instance.allItemsList.itemsList[ID];

    public CompatibleNoun? CreateOption()
    {
        var item = GetItem();
                
        // Skip if item invalid
        if (item == null)
            return null;
                
        var option = ScriptableObject.CreateInstance<TerminalKeyword>();
        option.word = item.itemName.ToLower();
    
        var success = ScriptableObject.CreateInstance<SuccessNode>();
        success.selectedItem = this;
            
        return new CompatibleNoun
        {
            noun = option,
            result = success
        };
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref SCRAP_VALUE);
        serializer.SerializeValue(ref SAVE_DATA);
    }
}