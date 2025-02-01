using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using ShipInventory.Extensions;
using ShipInventory.Helpers;
using ShipInventory.Items;
using Unity.Netcode;
using UnityEngine;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Objects;

public class ChuteInteract : NetworkBehaviour
{
    public static ChuteInteract? Instance;

    #region Store

    private void StoreHeldItem(PlayerControllerB player)
    {
        GrabbableObject item = player.currentlyHeldObjectServer;

        // If item invalid, skip
        if (item is null)
        {
            Logger.Info($"Player '{player.playerUsername}' is not holding any item.");
            return;
        }

        StoreItem(item);

        // Update scrap collected
        item.isInShipRoom = false;
        item.scrapPersistedThroughRounds = true;
        player.SetItemInElevator(true, true, item);

        // Despawn the held item
        Logger.Debug("Despawn held object...");
        player.DestroyItemInSlotAndSync(player.currentItemSlot);
    }

    private void StoreItem(GrabbableObject item)
    {
        var data = ConvertItemHelper.ConvertItem(item);

        // Send store request to server
        StoreItems(data);
    }

    public void StoreItems(params ItemData[] items)
    {
        Logger.Debug($"Sending {items.Length} new items...");
        StoreItemsServerRpc(items, GetClientID());
        
        if (Compatibility.OpenMonitors.Enabled)
            Compatibility.OpenMonitors.UpdateMonitor();
    }
    
    #endregion

    #region Retrieve

    public void RetrieveItems(params ItemData[] items) => RetrieveItemsServerRpc(items, GetClientID());

    #endregion

    #region Spawn

    private Transform itemRestorePoint = null!;
    private ParticleSystem? spawnParticles;
    
    private readonly Queue<ItemData> spawnQueue = [];
    private Coroutine? spawnCoroutine;

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator SpawnCoroutine()
    {
        while (spawnQueue.Count > 0)
        {
            // If chute is full, skip
            if (itemsInChute.Length >= ShipInventory.Configuration.StopAfter.Value)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            
            var data = spawnQueue.Dequeue();
            var obj = SpawnItemServer(data);
            
            if (obj != null)
                SpawnItemClientRpc(obj, data);
            
            yield return new WaitForSeconds(ShipInventory.Configuration.TimeToRetrieve.Value);
        }
        
        // Mark as completed
        spawnCoroutine = null;
    }

    private NetworkObject? SpawnItemServer(ItemData data)
    {
        var item = data.GetItem();

        if (item == null)
        {
            Logger.Debug($"Tried to spawn '{data.ID}', but no item could have been found for it.");
            return null;
        }

        if (item.spawnPrefab == null)
        {
            Logger.Debug($"Cannot spawn '{data.ID}', because no prefab is assigned to it.");
            return null;
        }

        var obj = Instantiate(item.spawnPrefab, itemRestorePoint, false);
        
        if (obj == null)
            return null;
        
        if (!obj.TryGetComponent(out NetworkBehaviour behaviour))
            return null;
        
        var networkObj = behaviour.NetworkObject;
        networkObj.Spawn();

        return networkObj;
    }

    [ClientRpc]
    private void SpawnItemClientRpc(NetworkObjectReference networkObject, ItemData data)
    {
        if (!networkObject.TryGet(out var obj))
            return;
        
        // Set values
        obj.transform.SetParent(itemRestorePoint, false);
        obj.transform.position = itemRestorePoint.position;
        obj.transform.rotation = itemRestorePoint.rotation;
        
        if (!obj.TryGetComponent(out GrabbableObject grabObj))
            return;
        
        if (grabObj.itemProperties.itemSpawnsOnGround)
            obj.transform.localRotation = Quaternion.Euler(grabObj.itemProperties.restingRotation);
        else
            grabObj.OnHitGround();
        
        // Set up object
        grabObj.scrapPersistedThroughRounds = data.PERSISTED_THROUGH_ROUNDS;

        if (grabObj is BadItem badItem)
            badItem.ID = data.ID;
        
        var item = data.GetItem();

        if (item.isScrap)
            grabObj.SetScrapValue(data.SCRAP_VALUE);
            
        if (item.saveItemVariable)
            grabObj.LoadItemSaveData(data.SAVE_DATA);
        
        grabObj.isInShipRoom = true;
        grabObj.isInElevator = true;
        grabObj.StartCoroutine(PlayDropSound(grabObj));
        
        // Play particles
        spawnParticles?.Play();
    }
    
    private static IEnumerator PlayDropSound(GrabbableObject grabbable)
    {
        yield return null; // Wait for Awake
        yield return null; // Wait for Start
        grabbable.PlayDropSFX();
        grabbable.OnHitGround();
    }

    #endregion

    #region Server RPC

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

    [ServerRpc(RequireOwnership = false)]
    private void StoreItemsServerRpc(ItemData[] items, params ulong[] ids)
    {
        Logger.Debug($"Adding {items.Length} new items...");
        ItemManager.AddItems(items);
        Logger.Debug($"The inventory has been updated: '{ItemManager.GetKey()}'.");

        if (!ShipInventory.Configuration.ForceUpdateUponAdding.Value)
            return;
        
        ForceUpdateClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = ids
            }
        });
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RetrieveItemsServerRpc(ItemData[] items, params ulong[] ids)
    {
        var filteredItems = ItemManager.FilterExtras(items);

        if (filteredItems.Length != items.Length)
            Logger.Debug("Tried to retrieve an item that is not in the inventory anymore.");
        
        if (filteredItems.Length == 0)
            return;
        
        Logger.Debug($"Removing '{filteredItems.Length}' items...");
        ItemManager.RemoveItems(filteredItems);

        // Queue items
        foreach (var i in filteredItems)
            spawnQueue.Enqueue(i);

        spawnCoroutine ??= StartCoroutine(SpawnCoroutine());
        Logger.Debug($"{filteredItems.Length} items enqueued!");
        
        if (!ShipInventory.Configuration.ForceUpdateUponRemoving.Value)
            return;
        ForceUpdateClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = ids
            }
        });
    }

    #endregion

    #region Client RPC

    private string? updateKey;

    private static ulong GetClientID(PlayerControllerB? player = null) => (player ?? GameNetworkManager.Instance.localPlayerController).actualClientId;

    [ClientRpc]
    private void UpdateInventoryClientRpc(string? _updateKey, string? key, ItemData[] data, ClientRpcParams routing = default)
    {
        if (updateKey != _updateKey)
        {
            Logger.Debug("Received an update that was not the one expected.");
            return;
        }
        
        Logger.Debug($"Updating the cache with: '{key ?? "null"}'");
        ItemManager.UpdateCache(key, data.ToList());
        Logger.Debug("Cached updated!");

        updateKey = null;
    }

    [ClientRpc]
    private void InventoryUpToDateClientRpc(string? _updateKey, ClientRpcParams routing = default)
    {
        if (updateKey != _updateKey)
        {
            Logger.Debug("Received an inventory check that was not the one expected.");
            return;
        }

        updateKey = null;
        
        if (ShipInventory.Configuration.InventoryUpdateCheckSilencer.Value)
            return;
        
        Logger.Debug($"The inventory is up to date! ({ItemManager.GetKey()})");
    }

    [ClientRpc]
    private void ForceUpdateClientRpc(ClientRpcParams routing = default) => StartNewCheck();

    #endregion
    
    #region Trigger

    private Collider[] itemsInChute = [];
    private InteractTrigger _trigger = null!;

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateTrigger()
    {
        // If no trigger, skip
        if (!_trigger)
            return;
        
        if (GameNetworkManager.Instance is null)
            return;
        
        if (NetworkManager.Singleton is null)
            return;
        
        var local = GameNetworkManager.Instance.localPlayerController;
        
        // If player invalid, skip
        if (local is null)
            return;
        
        // If player outside the ship, skip
        if (!local.isInHangarShipRoom)
            return;

        // Update interactable
        _trigger.UpdateChuteTrigger(local);

        // ReSharper disable Unity.PreferNonAllocApi
        itemsInChute = Physics.OverlapBox(
            itemRestorePoint.position,
            new Vector3(1f, 0.25f, 1.25f) / 2,
            itemRestorePoint.rotation,
            1 << LAYER_PROPS
        );

        // Update layer
        gameObject.layer = itemsInChute.Length > 0 ? LAYER_IGNORE : LAYER_INTERACTABLE;
    }

    #endregion

    #region Unlockable

    public static UnlockableItem? UnlockableItem;

    public static void SetOffsets(AutoParentToShip autoParent)
    {
        autoParent.positionOffset = new Vector3(1.9f, 1f, -4.5f);
        autoParent.rotationOffset = new Vector3(35, 0, 0);
    }

    #endregion

    #region Update

    private Coroutine? updateCoroutine;

    private void StartNewCheck()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
        
        updateCoroutine = StartCoroutine(UpdateInventory());
    }
    
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
            CheckInventoryServerRpc(updateKey, ItemManager.GetKey(), GetClientID(player));
            
            yield return new WaitForSeconds(ShipInventory.Configuration.InventoryRefreshRate.Value);
        }
    }

    #endregion

    #region MonoBehaviour

    private int LAYER_IGNORE = -1; 
    private int LAYER_INTERACTABLE = -1;
    private int LAYER_PROPS = -1;
    
    private void Start()
    {
        LAYER_IGNORE = LayerMask.NameToLayer(Constants.LAYER_IGNORE);
        LAYER_INTERACTABLE = LayerMask.NameToLayer(Constants.LAYER_INTERACTABLE);
        LAYER_PROPS = LayerMask.NameToLayer(Constants.LAYER_PROPS);
        
        _trigger = GetComponent<InteractTrigger>();
        _trigger.onInteract.AddListener(StoreHeldItem);
        _trigger.timeToHold = ShipInventory.Configuration.TimeToStore.Value;

        itemRestorePoint = transform.Find(Constants.DROP_NODE_PATH);
        spawnParticles = GetComponentInChildren<ParticleSystem>();
        
        Instance = this;
        StartNewCheck();
    }

    private void Update() => UpdateTrigger();

    public override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;

        if (StartOfRound.Instance.firingPlayersCutsceneRunning && !ShipInventory.Configuration.PersistThroughFire.Value)
        {
            Logger.Debug("Clearing cache from fire!");
            ItemManager.ClearCache();
        }
    }

    #endregion
}