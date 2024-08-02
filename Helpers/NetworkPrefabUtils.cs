using System;
using System.Collections.Generic;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace ShipInventory.Helpers;

public static class NetworkPrefabUtils
{
    private static readonly Dictionary<string, Action<GameObject>?> prefabsToAdd = [];
    private static readonly Dictionary<string, GameObject> prefabsLoaded = [];

    public static void LoadPrefab(string name, Action<GameObject>? callback = null) 
        => prefabsToAdd.Add(name, callback);

    public static GameObject? GetPrefab(string name) => prefabsLoaded.GetValueOrDefault(name);

    #region Patches

    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManager_Patches
    {
        [HarmonyPatch(nameof(GameNetworkManager.Start))]
        private static void Postfix(GameNetworkManager __instance)
        {
            foreach (var (name, callback) in prefabsToAdd)
            {
                var prefab = Bundle.LoadAsset<GameObject>(name);

                if (prefab is null)
                    continue;
            
                callback?.Invoke(prefab);
                NetworkManager.Singleton.AddNetworkPrefab(prefab);
                prefabsLoaded.Add(name, prefab);
            }
        
            prefabsToAdd.Clear();
        }
    }
    
    #endregion
}

