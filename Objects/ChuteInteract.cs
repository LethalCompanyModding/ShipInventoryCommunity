using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Objects;

public class ChuteInteract : NetworkBehaviour
{
    public static ChuteInteract Instance = null!;

    public override void OnNetworkSpawn()
    {
        _trigger = GetComponent<InteractTrigger>();
        itemRestorePoint = transform.Find("DropNode");
        spawnParticles = GetComponentInChildren<ParticleSystem>();
        
        base.OnNetworkSpawn();
    }

    #region Items

    private static IEnumerable<ItemData> storedItems = [];
    public static IEnumerable<ItemData> GetItems() => storedItems.OrderBy(i => i.GetItem()?.itemName).ToList();

    internal static void SetItems(IEnumerable<ItemData> newItems)
    {
        storedItems = newItems;
        UpdateValue();
    }

    private static void Add(ItemData data) => SetItems(storedItems.Append(data));

    private static void Remove(ItemData data)
    {
        var copy = storedItems.ToList();
        copy.Remove(data);
        SetItems(copy);
    }

    public static void UpdateValue()
    {
        if (Instance == null)
            return;
        
        var grabObj = Instance.GetComponent<GrabbableObject>();
        
        if (grabObj == null)
            return;

        grabObj.scrapValue = storedItems.Sum(i => i.SCRAP_VALUE);
        grabObj.OnHitGround(); // Update 
    }

    #endregion
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
        
        ItemData data = ItemData.Save(item);
        
        // Despawn the held item
        Logger.Debug("Despawn held object...");
        player.DespawnHeldObject();
        
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
        Add(data);
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

        // If item not stored, skip
        var allData = storedItems.Where(d => d.ID == data.ID).ToList();

        if (!allData.Any())
        {
            Logger.Info($"The chute does not contain any '{item.itemName}'.");
            return;
        }
        
        // Select data to spawn
        count = Mathf.Min(allData.Count, count);
        var selectedData = allData.GetRange(0, count);

        // Spawn each item
        foreach (var singleData in selectedData)
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
        
        Logger.Debug($"Server spawned {count} new items!");
    }

    [ClientRpc]
    public void SpawnItemClientRpc(NetworkObjectReference networkObject, ItemData data)
    {
        Logger.Debug("Updating the items...");
        Remove(data);
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
        RequestItemsServerRpc(StartOfRound.Instance.localPlayerController.playerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestItemsServerRpc(ulong playerId)
    {
        Logger.Debug("Item request heard!");
        Logger.Debug($"Sending the items to client {playerId}...");
        RequestItemsClientRpc(storedItems.ToArray(), new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = [playerId]
            }
        });
    }

    [ClientRpc]
    private void RequestItemsClientRpc(ItemData[] data, ClientRpcParams @params = default)
    {
        Logger.Debug("Client received items!");
        
        // Skip if target is invalid
        var targets = @params.Send.TargetClientIds;
        
        if (targets != null)
            return;

        SetItems(data.ToList());
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
        
        // If player invalid, skip
        if (GameNetworkManager.Instance?.localPlayerController is null)
            return;

        // Update interactable
        _trigger.interactable = GameNetworkManager.Instance.localPlayerController.isHoldingObject;

        // Update layer
        var hasItem = Physics.CheckSphere(itemRestorePoint.position, 0.2f, 1 << LayerMask.NameToLayer(Constants.LAYER_PROPS));
        
        gameObject.layer = LayerMask.NameToLayer(hasItem ? Constants.LAYER_IGNORE : Constants.LAYER_INTERACTABLE);
    }

    #endregion
    #region MonoBehaviour

    private void Update() => UpdateTrigger();

    #endregion
}