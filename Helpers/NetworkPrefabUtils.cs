using System;
using System.Collections.Generic;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace ShipInventory.Helpers;

/// <summary>
/// Helper that handles to register and loads prefabs
/// </summary>
internal static class NetworkPrefabUtils
{
    private class PrefabData
    {
        public Action<GameObject>? onLoad;
        public GameObject? gameObject;
        public bool isUnlockable;
    }

    private static readonly Dictionary<string, PrefabData> prefabs = [];

    public static void Register(string name, Action<GameObject>? onLoad = null, bool isUnlockable = false)
    {
        if (prefabs.ContainsKey(name))
        {
            Logger.Error($"Tried to register multiple prefabs with the name '{name}'.");
            return;
        }
        
        prefabs.Add(name, new PrefabData
        {
            onLoad = onLoad,
            isUnlockable = isUnlockable
        });
    }
        
    private static GameObject? Get(string name) => prefabs.TryGetValue(name, out var data) ? data.gameObject : null;

    private static GameObject? AddToNetwork(string name, PrefabData data)
    {
        var prefab = Bundle.LoadAsset<GameObject>(name);

        if (prefab is null)
            return null;

        data.onLoad?.Invoke(prefab);
        NetworkManager.Singleton.AddNetworkPrefab(prefab);
        return prefab;
    }
    
    private static void Setup(Transform parent, string name, bool isHost)
    {
        if (isHost)
            Spawn(parent, name);
    }

    private static void Spawn(Transform parent, string name)
    {
        var prefab = Get(name);

        if (prefab == null)
        {
            Logger.Error($"The prefab '{name}' was not found in the bundle!");
            return;
        }

        var obj = UnityEngine.Object.Instantiate(prefab);

        if (obj == null)
        {
            Logger.Error($"Could not create a new instance of '{name}'!");
            return;
        }

        var networkObj = obj.GetComponent<NetworkBehaviour>().NetworkObject;
        networkObj.Spawn();
        networkObj.TrySetParent(parent);
    }
    
    #region Patches

    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManager_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameNetworkManager.Start))]
        private static void AddPrefabsToNetwork(GameNetworkManager __instance)
        {
            foreach (var (name, data) in prefabs)
                data.gameObject = AddToNetwork(name, data);
        }
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRound_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StartOfRound.Start))]
        private static void SetUpPrefabs(StartOfRound __instance)
        {
            Transform parent = GameObject.Find(Constants.SHIP_PATH).transform;
            bool isHost = __instance.IsServer || __instance.IsHost;

            foreach (var (name, data) in prefabs)
            {
                if (!data.isUnlockable)
                    Setup(parent.transform, name, isHost);
            }
        }
    }

    #endregion
}