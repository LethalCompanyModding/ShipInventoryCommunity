using System;
using System.Linq;
using GameNetcodeStuff;
using ShipInventory.Helpers;
using Unity.Netcode;
using UnityEngine;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Objects;

public class ChuteInteract : NetworkBehaviour
{
    public static ChuteInteract? Instance = null;

    public override void OnNetworkSpawn()
    {
        _trigger = GetComponent<InteractTrigger>();
        itemRestorePoint = transform.Find("DropNode");
        spawnParticles = GetComponentInChildren<ParticleSystem>();
        
        base.OnNetworkSpawn();
    }

    #region Store Items
    
    public void StoreHeldItem(PlayerControllerB player)
    {
        GrabbableObject item = player.currentlyHeldObjectServer;
        
        // If item invalid, skip
        if (item == null)
        {
            Logger.Info($"Player '{player.playerUsername}' is not holding any item.");
            return;
        }
        
        ItemData data = ItemManager.Save(item);
        
        // Despawn the held item
        Logger.Debug("Despawn held object...");
        player.DespawnHeldObject();
        
        // Destroy radar icon
        if (item.radarIcon != null)
            Destroy(item.radarIcon.gameObject);
        
        // Send store request to server
        Logger.Debug("Sending new item to server...");
        StoreItemServerRpc(data);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StoreItemServerRpc(ItemData data)
    {
        Logger.Debug("Server received new item!");
        Logger.Debug("Sending new item to clients...");
        StoreItemClientRpc(data);
    }

    [ClientRpc]
    private void StoreItemClientRpc(ItemData data)
    {
        Logger.Debug("Client received new item!");
        ItemManager.Add(data);
        Logger.Debug("Client added new item!");
    }

    #endregion
    #region Spawn Items

    private Transform itemRestorePoint = null!;
    private ParticleSystem spawnParticles = null!;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnItemServerRpc(ItemData data, int count = 1)
    {
        var item = data.GetItem();
        
        if (item is null)
            return;

        var items = ItemManager.GetInstances(data, count);

        // Spawn each item
        foreach (var singleData in items)
        {
            var newItem = Instantiate(item.spawnPrefab) ?? throw new NullReferenceException();
            newItem.transform.SetParent(GameObject.Find(Constants.SHIP_PATH).transform, false);
            
            // Set values
            var grabObj = newItem.GetComponent<GrabbableObject>();
            
            // Call spawn methods
            grabObj.Start();
            grabObj.PlayDropSFX();
            
            // Spawn item
            var networkObj = grabObj.NetworkObject;
            networkObj.Spawn();
            
            SpawnItemClientRpc(networkObj, singleData);
        }
        
        Logger.Debug($"Server spawned {items.Count()} new items!");
    }

    [ClientRpc]
    public void SpawnItemClientRpc(NetworkObjectReference networkObject, ItemData data)
    {
        Logger.Debug("Updating the items...");
        ItemManager.Remove(data);
        Logger.Debug("Items updated!");
        
        var item = data.GetItem();

        if (!networkObject.TryGet(out var obj) || item == null)
            return;

        var grabObj = obj.GetComponent<GrabbableObject>();
        
        // Set up object
        grabObj.parentObject = itemRestorePoint;
        
        if (item.isScrap)
            grabObj.SetScrapValue(data.SCRAP_VALUE);
            
        if (item.saveItemVariable)
            grabObj.LoadItemSaveData(data.SAVE_DATA);

        grabObj.isInShipRoom = true;
        grabObj.isInElevator = true;
        grabObj.OnHitGround();

        // Play particles
        spawnParticles.Play();
        
        Logger.Debug("Item setup!");
    }

    #endregion
    #region Request Items

    public void RequestItems()
    {
        Logger.Debug("Requesting the items to the server...");
        RequestItemsServerRpc(GameNetworkManager.Instance.localPlayerController.playerClientId);
    }

    public void RequestAll()
    {
        // Skip if request from client
        if (GameNetworkManager.Instance.localPlayerController.IsClient)
            return;

        var ids = StartOfRound.Instance.allPlayerScripts
            .Where(p => p.IsClient)
            .Select(p => p.playerClientId);
        
        RequestItemsServerRpc(ids.ToArray());
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestItemsServerRpc(params ulong[] ids)
    {
        Logger.Debug("Item request heard!");
        Logger.Debug($"Sending the items to client {string.Join(", ", ids)}...");
        RequestItemsClientRpc(ItemManager.GetItems().ToArray(), new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = ids
            }
        });
    }

    [ClientRpc]
    private void RequestItemsClientRpc(ItemData[] data, ClientRpcParams @params = default)
    {
        Logger.Debug("Client received items!");
        
        // Skip if target is invalid
        var targets = @params.Send.TargetClientIds ?? [];
        
        // If not a target, skip
        if (!targets.Contains(GameNetworkManager.Instance.localPlayerController.playerClientId))
            return;

        ItemManager.SetItems(data.ToList());
        Logger.Debug("Client updated items!");
    }

    #endregion
    #region Trigger

    private InteractTrigger _trigger = null!;

    private void UpdateTrigger()
    {
        if (NetworkManager.Singleton is null)
            return;
        
        // If no trigger, skip
        if (!_trigger)
            return;

        var local = GameNetworkManager.Instance?.localPlayerController;
        
        // If player invalid, skip
        if (local is null)
            return;

        // Update interactable
        var isAllowed = ItemManager.IsItemAllowed(local.currentlyHeldObjectServer?.itemProperties);
        _trigger.interactable = local.isHoldingObject && isAllowed;
        _trigger.disabledHoverTip = isAllowed ? Constants.NOT_HOLDING_ITEM : Constants.ITEM_NOT_ALLOWED;

        // Update layer
        var hasItem = Physics.CheckSphere(itemRestorePoint.position, 0.2f, 1 << LayerMask.NameToLayer(Constants.LAYER_PROPS));
        
        gameObject.layer = LayerMask.NameToLayer(hasItem ? Constants.LAYER_IGNORE : Constants.LAYER_INTERACTABLE);
    }

    #endregion
    #region MonoBehaviour

    private void Update() => UpdateTrigger();

    #endregion
}