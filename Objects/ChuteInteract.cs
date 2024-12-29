using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using ShipInventory.Helpers;
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
        // Send store request to server
        ItemData data = new ItemData(item);

        item.OnBroughtToShip();

        StoreItems(data);
    }

    public void StoreItems(params ItemData[] items)
    {
        Logger.Debug($"Sending {items.Length} new items...");
        StoreItemsServerRpc(items, GetClientID());
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
        if (_placeableShipObject != null)
            _placeableShipObject.inUse = true;
        
        while (spawnQueue.Count > 0)
        {
            // If chute is full, skip
            if (itemsInChute.Length >= ShipInventory.Config.StopAfter.Value)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            
            var data = spawnQueue.Dequeue();
            var obj = SpawnItemServer(data);
            
            if (obj != null)
                SpawnItemClientRpc(obj, data);
            
            yield return new WaitForSeconds(ShipInventory.Config.TimeToRetrieve.Value);
        }
        
        // Mark as completed
        spawnCoroutine = null;
        
        if (_placeableShipObject != null)
            _placeableShipObject.inUse = false;
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
        
        // Set values
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        
        if (obj.TryGetComponent(out GrabbableObject grabObj))
        {
            if (grabObj.itemProperties.itemSpawnsOnGround)
                obj.transform.localRotation = Quaternion.Euler(grabObj.itemProperties.restingRotation);
            else
                grabObj.OnHitGround();
        }
        
        // Spawn on network
        var networkObj = grabObj.NetworkObject;
        networkObj.Spawn();

        return networkObj;
    }

    [ClientRpc]
    private void SpawnItemClientRpc(NetworkObjectReference networkObject, ItemData data)
    {
        if (!networkObject.TryGet(out var obj))
            return;
        
        if (!obj.TryGetComponent(out GrabbableObject grabObj))
            return;
        
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
        var target = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = ids
            }
        };

        var localKey = ItemManager.GetKey();
        
        // Check if cache is deprecated
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
        Logger.Debug($"The inventory has been updated: '{ItemManager.GetKey() ?? "null"}'.");

        if (ShipInventory.Config.ForceUpdateUponAdding.Value)
        {
            ForceUpdateClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = ids
                }
            });
        }
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
        
        if (ShipInventory.Config.ForceUpdateUponRemoving.Value)
        {
            ForceUpdateClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = ids
                }
            });
        }
    }

    #endregion

    #region Client RPC

    private string? updateKey;

    private static ulong GetClientID() => GameNetworkManager.Instance.localPlayerController.playerClientId;

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
            return;

        updateKey = null;
        
        if (ShipInventory.Config.InventoryUpdateCheckSilencer.Value)
            return;
        
        Logger.Debug("The inventory is up to date!");
    }

    [ClientRpc]
    private void ForceUpdateClientRpc(ClientRpcParams routing = default) => UpdateInventory();

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

    public static bool IsUpgrade;

    private PlaceableShipObject? _placeableShipObject;

    public static void SetOffsets(AutoParentToShip autoParent)
    {
        autoParent.positionOffset = new Vector3(1.9f, 1f, -4.5f);
        autoParent.rotationOffset = new Vector3(35, 0, 0);
    }

    #endregion

    #region MonoBehaviour

    private int LAYER_IGNORE = -1; 
    private int LAYER_INTERACTABLE = -1;
    private int LAYER_PROPS = -1;
    
    // Ask server to check cache
    private void UpdateInventory()
    {
        if (GameNetworkManager.Instance?.localPlayerController == null)
            return;

        updateKey = System.Guid.NewGuid().ToString();
        CheckInventoryServerRpc(updateKey, ItemManager.GetKey(), GetClientID());
    }

    private void Start()
    {
        LAYER_IGNORE = LayerMask.NameToLayer(Constants.LAYER_IGNORE);
        LAYER_INTERACTABLE = LayerMask.NameToLayer(Constants.LAYER_INTERACTABLE);
        LAYER_PROPS = LayerMask.NameToLayer(Constants.LAYER_PROPS);
        
        _trigger = GetComponent<InteractTrigger>();
        _trigger.onInteract.AddListener(StoreHeldItem);
        _trigger.timeToHold = ShipInventory.Config.TimeToStore.Value;

        itemRestorePoint = transform.Find(Constants.DROP_NODE_PATH);
        spawnParticles = GetComponentInChildren<ParticleSystem>();

        _placeableShipObject = GetComponentInChildren<PlaceableShipObject>();
        
        Instance = this;
    }

    private void Update() => UpdateTrigger();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        InvokeRepeating(nameof(UpdateInventory), 0, ShipInventory.Config.InventoryRefreshRate.Value);
    }

    #endregion
}