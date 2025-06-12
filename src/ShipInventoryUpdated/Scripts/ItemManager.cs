using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Objects;
using Unity.Netcode;

namespace ShipInventoryUpdated.Scripts;

public class ItemManager : NetworkBehaviour
{
    private readonly NetworkList<ItemData> storedItems = new(
        null,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    public delegate void OnListChanged(ItemData value);
    
    public OnListChanged? OnAdded;
    public OnListChanged? OnRemoved;

    /// <inheritdoc/>
    public override void OnNetworkSpawn()
    {
        storedItems.OnListChanged += @event =>
        {
            var value = @event.Value;
            
            switch (@event.Type)
            {
                case NetworkListEvent<ItemData>.EventType.Add:
                    OnAdded?.Invoke(value);
                    break;
                case NetworkListEvent<ItemData>.EventType.Remove:
                    OnRemoved?.Invoke(value);
                    break;
            }
        };

        OnAdded += i => Logger.Info("Item added: " + i.ID);
    }

    /// <summary>
    /// Adds the given items to the inventory
    /// </summary>
    public void Add(GrabbableObject item)
    {
        //var data = ConvertItemHelper.ConvertItem(item);
        var data = new ItemData(item);
        
        AddServerRpc(data);
    }

    #region RPC

    [ServerRpc(RequireOwnership = false)]
    private void AddServerRpc(params ItemData[] items)
    {
        foreach (var item in items)
            storedItems.Add(item);
    }
    
    #endregion
}