using System;
using System.Collections;
using System.Collections.Generic;
using ShipInventoryUpdated.Extensions;
using ShipInventoryUpdated.Items;
using Unity.Netcode;
using UnityEngine;

namespace ShipInventoryUpdated.Objects;

/// <summary>
/// Script that enables players to retrieve items in the chute
/// </summary>
public class ChuteRetrieveScript : NetworkBehaviour
{
    #region Unity

    private int LAYER_IGNORE = -1; 
    private int LAYER_INTERACTABLE = -1;
    private int LAYER_PROPS = -1;

    private void Start()
    {
        LAYER_IGNORE = LayerMask.NameToLayer(Constants.LAYER_IGNORE);
        LAYER_INTERACTABLE = LayerMask.NameToLayer(Constants.LAYER_INTERACTABLE);
        LAYER_PROPS = LayerMask.NameToLayer(Constants.LAYER_PROPS);

        dropShipTransform = GameObject.Find("/Environment/HangarShip").transform;
    }

    private void FixedUpdate()
    {
        if (itemRestorePoint == null)
            return;

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

    #region Server

    /// <summary>
    /// Requests the server to retrieve the given items
    /// </summary>
    public void RetrieveItems(params ItemData[] items)
    {
        var clientId = GameNetworkManager.Instance.localPlayerController.actualClientId;
        RetrieveItemsServerRpc(items, clientId);
    }

    /// <summary>
    /// Spawns the given item on the server
    /// </summary>
    private NetworkObject? SpawnItemServer(ItemData data)
    {
        var item = data.GetItem();

        if (item == null)
        {
            Helpers.Logger.Debug($"Tried to spawn '{data.ID}', but no item could have been found for it.");
            return null;
        }

        if (item.spawnPrefab == null)
        {
            Helpers.Logger.Debug($"Cannot spawn '{data.ID}', because no prefab is assigned to it.");
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

    #endregion

    #region Coroutine

    private readonly Queue<ItemData> spawnQueue = [];
    private Coroutine? spawnCoroutine;

    /// <summary>
    /// Plays the dropping sound of the given item
    /// </summary>
    private IEnumerator SpawnCoroutine()
    {
        while (spawnQueue.Count > 0)
        {
            // If chute is full, skip
            if (itemsInChute.Length >= 30)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            
            var data = spawnQueue.Dequeue();
            var obj = SpawnItemServer(data);
            
            if (obj != null)
                SpawnItemClientRpc(obj, data);
            
            yield return new WaitForSeconds(0.5f);
        }
        
        // Mark as completed
        spawnCoroutine = null;
    }

    /// <summary>
    /// Plays the dropping sound of the given item
    /// </summary>
    private static IEnumerator PlayDropSound(GrabbableObject grabbable)
    {
        yield return null; // Wait for Awake
        yield return null; // Wait for Start
        grabbable.PlayDropSFX();
        grabbable.OnHitGround();
    }

    #endregion

    #region RPCs

    [SerializeField] private Transform itemRestorePoint = null!;
    [SerializeField] private ParticleSystem? spawnParticles;
    [SerializeField] private ChuteUpdateScript? updateScript;

    private Collider[] itemsInChute = [];
    private Transform dropShipTransform;

    /// <summary>
    /// Spawns the given items across all clients
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void RetrieveItemsServerRpc(ItemData[] items, params ulong[] ids)
    {
        var filteredItems = ItemManager.FilterExtras(items);

        if (filteredItems.Length != items.Length)
            Helpers.Logger.Debug("Tried to retrieve an item that is not in the inventory anymore.");
        
        if (filteredItems.Length == 0)
            return;
        
        Helpers.Logger.Debug($"Removing '{filteredItems.Length}' items...");
        ItemManager.RemoveItems(filteredItems);

        // Queue items
        foreach (var i in filteredItems)
            spawnQueue.Enqueue(i);

        spawnCoroutine ??= StartCoroutine(SpawnCoroutine());
        Helpers.Logger.Debug($"{filteredItems.Length} items enqueued!");
        
        updateScript?.ForceUpdateClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = ids
            }
        });
    }

    /// <summary>
    /// Spawns the given item with the given data
    /// </summary>
    [ClientRpc]
    private void SpawnItemClientRpc(NetworkObjectReference networkObject, ItemData data)
    {
        if (!networkObject.TryGet(out var obj))
            return;

        // Set values
        // Set the parent to the Drop Ship's transform
        try
        {
            obj.transform.SetParent(dropShipTransform, false);
        }
        catch
        {
            //Fallback should an error occur when parenting the object to the Drop Ship's transform
            Helpers.Logger.Error("Failed to parent the object to the Drop Ship's transform.");
            obj.transform.SetParent(itemRestorePoint, false);

        }
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

    #endregion
}