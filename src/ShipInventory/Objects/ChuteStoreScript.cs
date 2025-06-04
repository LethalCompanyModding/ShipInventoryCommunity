using GameNetcodeStuff;
using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Items;
using Unity.Netcode;
using UnityEngine;

namespace ShipInventoryUpdated.Objects;

/// <summary>
/// Script that enables players to store items in the chute
/// </summary>
public class ChuteStoreScript : NetworkBehaviour
{
    #region Unity

    private void Start()
    {
        if (trigger != null)
        {
            trigger.onInteract.AddListener(StoreHeldItem);
            trigger.timeToHold = 0.5f;
        }
    }

    #endregion

    #region Store

    [SerializeField] private ChuteUpdateScript? updateScript;
    [SerializeField] private InteractTrigger? trigger;

    /// <summary>
    /// Sends the item held by the given player to the server to be stored
    /// </summary>
    private void StoreHeldItem(PlayerControllerB player)
    {
        GrabbableObject item = player.currentlyHeldObjectServer;

        // If item invalid, skip
        if (item == null)
        {
            Helpers.Logger.Info($"Player '{player.playerUsername}' is not holding any item.");
            return;
        }

        var data = ConvertItemHelper.ConvertItem(item);
        StoreItems(data);

        // Update scrap collected
        item.isInShipRoom = false;
        item.scrapPersistedThroughRounds = true;
        player.SetItemInElevator(true, true, item);

        // Despawn the held item
        Helpers.Logger.Debug("Despawn held object...");
        player.DestroyItemInSlotAndSync(player.currentItemSlot);
    }

    /// <summary>
    /// Sends the given data to be stored in the server
    /// </summary>
    public void StoreItems(params ItemData[] items)
    {
        Helpers.Logger.Debug($"Sending {items.Length} new items...");

        var clientId = GameNetworkManager.Instance.localPlayerController.actualClientId;
        StoreItemsServerRpc(items, clientId);
    }

    #endregion

    #region RPCs

    /// <summary>
    /// Stores the given items locally
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void StoreItemsServerRpc(ItemData[] items, params ulong[] ids)
    {
        Helpers.Logger.Debug($"Adding {items.Length} new items...");
        Items.ItemManager.AddItems(items);
        Helpers.Logger.Debug($"The inventory has been updated: '{ItemManager.GetKey()}'.");
        
        updateScript?.ForceUpdateClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = ids
            }
        });
    }

    #endregion
}