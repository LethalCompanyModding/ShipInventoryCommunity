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
    }

    private static readonly Dictionary<string, PrefabData> prefabs = [];

    public static void Register(string name, Action<GameObject>? onLoad = null)
    {
        if (prefabs.ContainsKey(name))
        {
            Logger.Error($"Tried to register multiple prefabs with the name '{name}'.");
            return;
        }
        
        prefabs.Add(name, new PrefabData
        {
            onLoad = onLoad
        });
    }
        
    private static void AddToNetwork(string name, PrefabData data)
    {
        var prefab = Bundle.LoadAsset<GameObject>(name);

        if (prefab is null)
            return;

        data.onLoad?.Invoke(prefab);
        NetworkManager.Singleton.AddNetworkPrefab(prefab);
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
                AddToNetwork(name, data);
        }
    }

    #endregion
}