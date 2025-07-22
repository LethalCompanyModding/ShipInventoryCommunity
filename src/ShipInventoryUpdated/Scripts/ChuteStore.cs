using GameNetcodeStuff;
using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Helpers.API;
using UnityEngine;

#pragma warning disable CS0649

namespace ShipInventoryUpdated.Scripts;

/// <summary>
/// Script that enables players to store items in the chute
/// </summary>
public class ChuteStore : MonoBehaviour
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

    [SerializeField] private InteractTrigger? trigger;

    /// <summary>
    /// Sends the item held by the given player to the server to be stored
    /// </summary>
    private static void StoreHeldItem(PlayerControllerB player)
    {
        GrabbableObject item = player.currentlyHeldObjectServer;

        // If item invalid, skip
        if (item == null)
        {
            Helpers.Logger.Info($"Player '{player.playerUsername}' is not holding any item.");
            return;
        }

        var data = ItemConverter.Convert(item);
        Inventory.Add(data);

        // Update scrap collected
        item.isInShipRoom = false;
        item.scrapPersistedThroughRounds = true;
        player.SetItemInElevator(true, true, item);

        // Despawn the held item
        Helpers.Logger.Debug("Despawn held object...");
        player.DestroyItemInSlotAndSync(player.currentItemSlot);
    }

    #endregion
}