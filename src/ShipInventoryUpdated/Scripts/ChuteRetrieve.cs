using System.Collections;
using System.Collections.Generic;
using ShipInventoryUpdated.Objects;
using Unity.Netcode;
using UnityEngine;
using Logger = ShipInventoryUpdated.Helpers.Logger;

namespace ShipInventoryUpdated.Scripts;

#pragma warning disable CS0649

/// <summary>
/// Script that enables players to retrieve items in the chute
/// </summary>
public class ChuteRetrieve : MonoBehaviour
{
    #region Fields

    [SerializeField] private Transform? itemRestorePoint;
    [SerializeField] private Vector3 itemDetectionSize;
    [SerializeField] private InteractTrigger? trigger;
    [SerializeField] private ParticleSystem? spawnParticles;

    #endregion

    #region Unity

    private int LAYER_IGNORE = -1; 
    private int LAYER_INTERACTABLE = -1;
    private int LAYER_PROPS = -1;

    private readonly Collider[] buffer = new Collider[1];

    private void Awake()
    {
        LAYER_IGNORE = LayerMask.NameToLayer("Ignore Raycast");
        LAYER_INTERACTABLE = LayerMask.NameToLayer("InteractableObject");
        LAYER_PROPS = LayerMask.NameToLayer("Props");
        
        Inventory.OnRemoved += RetrieveItemsServerRpc;
        Inventory.OnAdded += Inventory.Remove;
    }

    private void FixedUpdate()
    {
        if (itemRestorePoint is null || trigger is null)
            return;

        var amount = Physics.OverlapBoxNonAlloc(
            itemRestorePoint.position,
            itemDetectionSize / 2,
            buffer,
            itemRestorePoint.rotation,
            1 << LAYER_PROPS
        );
        
        trigger.gameObject.layer = amount > 0 ? LAYER_IGNORE : LAYER_INTERACTABLE;
    }

    private void OnDestroy()
    {
        Inventory.OnRemoved -= RetrieveItemsServerRpc;
    }

    #endregion
    
    #region Coroutines
    
    private readonly Queue<ItemData> spawnQueue = [];
    private Coroutine? spawnCoroutine;

    /// <summary>
    /// Plays the dropping sound of the given item
    /// </summary>
    private static IEnumerator PlayDropSound(GrabbableObject item)
    {
        yield return null; // Wait for Awake
        yield return null; // Wait for Start
        item.PlayDropSFX();
        item.OnHitGround();
    }
    
    /// <summary>
    /// Plays the dropping sound of the given item
    /// </summary>
    private IEnumerator SpawnCoroutine()
    {
        while (spawnQueue.Count > 0)
        {
            var data = spawnQueue.Dequeue();
            var obj = SpawnItemServer(data);
            
            if (obj is not null)
                SpawnItemClientRpc(obj, data);
            
            yield return new WaitForSeconds(0.5f);
        }
        
        // Mark as completed
        spawnCoroutine = null;
    }
    
    /// <summary>
    /// Spawns the given item on the server
    /// </summary>
    private NetworkObject? SpawnItemServer(ItemData data)
    {
        var item = data.GetItem();

        if (item is null)
        {
            Logger.Error($"Tried to spawn '{data.ID}', but no item could have been found for it.");
            return null;
        }

        if (item.spawnPrefab is null)
        {
            Logger.Debug($"Cannot spawn '{data.ID}', because no prefab is assigned to it.");
            return null;
        }

        var obj = Instantiate(item.spawnPrefab, itemRestorePoint, false);
        
        if (obj is null)
            return null;
        
        if (!obj.TryGetComponent(out NetworkBehaviour behaviour))
            return null;
        
        var networkObj = behaviour.NetworkObject;
        networkObj.Spawn();

        return networkObj;
    }

    #endregion

    #region RPCs

    /// <summary>
    /// Spawns the given item with the given data
    /// </summary>
    [ClientRpc]
    private void SpawnItemClientRpc(NetworkObjectReference networkObject, ItemData data)
    {
        var item = data.GetItem();

        if (item == null)
        {
            Logger.Error("Tried to spawn an item, but the original item was not found.");
            return;
        }

        if (itemRestorePoint == null)
        {
            Logger.Error("Tried to spawn an item, but no restore point was defined.");
            return;
        }

        if (!networkObject.TryGet(out var obj))
        {
            Logger.Error("Tried to spawn an item, but the original object was not found.");
            return;
        }

        obj.transform.SetParent(StartOfRound.Instance?.elevatorTransform ?? itemRestorePoint, false);
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
    
    /// <summary>
    /// Spawns the given items across all clients
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void RetrieveItemsServerRpc(ItemData[] items)
    {
        // Queue items
        foreach (var i in items)
            spawnQueue.Enqueue(i);
        
        spawnCoroutine ??= StartCoroutine(SpawnCoroutine());
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        if (itemRestorePoint == null)
            return;

        Gizmos.matrix = itemRestorePoint.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero,itemDetectionSize / 2);
    }

    #endregion
}