using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Objects;
using Unity.Netcode;

namespace ShipInventoryUpdated.Scripts;

public class Inventory : NetworkBehaviour
{
    private static Inventory? Instance;

    private readonly NetworkList<ItemData> storedItems = new(
        null,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    #region API
    
    public delegate void OnListChanged(ItemData[] value);
    
    public static OnListChanged? OnAdded;
    public static OnListChanged? OnRemoved;

    /// <summary>
    /// Adds the given items to the inventory
    /// </summary>
    public static void Add(GrabbableObject item)
    {
        if (Instance == null)
        {
            Logger.Warn("Tried to add an item to the inventory, but no instance was defined.");
            return;
        }
        
        var data = ItemConverter.Convert(item);
        
        Instance.AddServerRpc(data);
    }

    /// <summary>
    /// Removes the given items from the inventory
    /// </summary>
    /// <param name="items"></param>
    public static void Remove(ItemData[] items)
    {
        if (Instance == null)
        {
            Logger.Warn("Tried to remove an item to the inventory, but no instance was defined.");
            return;
        }

        Instance.RemoveServerRpc(items);
    }

    #endregion

    #region Unity

    /// <inheritdoc/>
    public override void OnNetworkSpawn()
    {
        Instance = this;
    }

    /// <inheritdoc/>
    public override void OnNetworkDespawn()
    {
        if (Instance == this)
            Instance = null;
    }

    #endregion
    
    #region RPC

    [ServerRpc(RequireOwnership = false)]
    private void AddServerRpc(params ItemData[] items)
    {
        foreach (var item in items)
            storedItems.Add(item);
        
        OnAdded?.Invoke(items);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveServerRpc(params ItemData[] items)
    {
        foreach (var item in items)
            storedItems.Remove(item);

        OnRemoved?.Invoke(items);
    }
    
    #endregion
}