using System;
using Unity.Netcode;
using UnityEngine;
using GameNetcodeStuff;
using System.Collections;
using ShipInventoryUpdated.Items;
using System.Linq;

namespace ShipInventoryUpdated.Objects;

/// <summary>
/// Script that enables the chute to automatically update itself
/// </summary>
public class ChuteUpdateScript : NetworkBehaviour
{
    #region Unity

    private void Start() => StartCheckCycle();

    #endregion

    #region Coroutine

    private string? updateKey;
    private Coroutine? updateCoroutine;

    /// <summary>
    /// Starts the cycle of updating
    /// </summary>
    private void StartCheckCycle()
    {
        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);
        
        updateCoroutine = StartCoroutine(UpdateInventory());
    }

    /// <summary>
    /// Checks every interval if the local inventory is out of date
    /// </summary>
    private IEnumerator UpdateInventory()
    {
        PlayerControllerB? player;

        do
        {
            player = GameNetworkManager.Instance?.localPlayerController;
            yield return null;
        } while (player == null);
        
        if (player.IsHost)
            yield break;

        while (true)
        {
            updateKey = System.Guid.NewGuid().ToString();
            CheckInventoryServerRpc(updateKey, ItemManager.GetKey(), player.actualClientId);
            
            yield return new WaitForSeconds(15);
        }
    }

    #endregion

    #region RPCs

    /// <summary>
    /// Requests the server to check if the given key is up to date
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void CheckInventoryServerRpc(string? _updateKey, string? key, params ulong[] ids)
    {
        ClientRpcParams target = default;
        target.Send.TargetClientIds = ids;

        var localKey = ItemManager.GetKey();
        
        // Check if client's cache is deprecated
        if (localKey != key)
        {
            UpdateInventoryClientRpc(_updateKey, localKey, ItemManager.GetItems(), target);
            return;
        }
        
        InventoryUpToDateClientRpc(_updateKey, target);
    }

    /// <summary>
    /// Updates the local inventory based on the received data
    /// </summary>
    [ClientRpc]
    private void UpdateInventoryClientRpc(string? _updateKey, string? key, ItemData[] data, ClientRpcParams routing = default)
    {
        if (updateKey != _updateKey)
        {
            Helpers.Logger.Debug("Received an update that was not the one expected.");
            return;
        }
        
        Helpers.Logger.Debug($"Updating the cache with: '{key ?? "null"}'");
        ItemManager.UpdateCache(key, data.ToList());
        Helpers.Logger.Debug("Cached updated!");

        updateKey = null;
    }

    /// <summary>
    /// Marks the local inventory as up to date
    /// </summary>
    [ClientRpc]
    private void InventoryUpToDateClientRpc(string? _updateKey, ClientRpcParams routing = default)
    {
        if (updateKey != _updateKey)
        {
            Helpers.Logger.Debug("Received an inventory check that was not the one expected.");
            return;
        }

        updateKey = null;
        
        Helpers.Logger.Debug($"The inventory is up to date! ({ItemManager.GetKey()})");
    }

    /// <summary>
    /// Forces the given clients to check if their local inventory is out of date
    /// </summary>
    [ClientRpc]
    public void ForceUpdateClientRpc(ClientRpcParams routing = default) => StartCheckCycle();

    #endregion
}