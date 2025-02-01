using ShipInventory.Compatibility;
using ShipInventory.Items;
using UnityEngine;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Objects;

public class BadItem : PhysicsProp
{
    public string? ID;
    
    public override void Start()
    {
        grabbable = true;
        grabbableToEnemies = true;
        isInFactory = true;
        
        base.Start();

        CheckForItem();
    }

    #region Data

    private ItemData? _data; // Data loaded from the save file
    internal int _index; // Index to the data of this item

    public override int GetItemDataToSave() => _index;

    public override void LoadItemSaveData(int saveData)
    {
        _index = saveData;
        
        if (!ItemsIO.GetItemKey(saveData, out _data) || _data == null)
            return;

        ID = _data.ID;
    }

    private void CheckForItem()
    {
        // If no data loaded, skip
        if (_data == null)
            return;
        
        var item = Compatibility.LethalLib.GetItem(_data.ID);
        
        // If item not found, skip
        if (item == null)
            return;
        
        Logger.Debug($"The item '{ID}' was found! Replacing the bad item with it...");
        
        GrabbableObject component = Instantiate(item.spawnPrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<GrabbableObject>();
        component.fallTime = 1f;
        component.hasHitGround = true;
        component.scrapPersistedThroughRounds = true;
        component.isInElevator = true;
        component.isInShipRoom = true;
        
        if (item.isScrap)
            component.SetScrapValue(_data.SCRAP_VALUE);
        
        if (item.saveItemVariable)
            component.LoadItemSaveData(_data.SAVE_DATA);
        
        component.NetworkObject.Spawn();
        
        NetworkObject.Despawn();
        
        Logger.Debug("The replacement was spawned and the bad item was despawned!");
    }
    
    #endregion
}