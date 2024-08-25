using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace ShipInventory.Helpers;

public static class NetworkPrefabUtils
{
    public class PrefabData
    {
        public string name = "";
        public Action<GameObject>? onLoad;
        public Action<GameObject>? onSetup;
        public GameObject? gameObject;
    }

    private static readonly List<PrefabData> prefabs = [];

    public static void LoadPrefab(PrefabData data) => prefabs.Add(data);

    public static GameObject? GetPrefab(string name) 
        => prefabs.FirstOrDefault(p => p.name == name)?.gameObject;

    #region Patches

    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManager_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameNetworkManager.Start))]
        private static void AddPrefabsToNetwork(GameNetworkManager __instance)
        {
            foreach (var data in prefabs)
            {
                var prefab = Bundle.LoadAsset<GameObject>(data.name);
                
                if (prefab is null)
                    continue;
                
                data.onLoad?.Invoke(prefab);
                NetworkManager.Singleton.AddNetworkPrefab(prefab);
                data.gameObject = prefab;
            }
        }
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRound_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StartOfRound.Start))]
        private static void SetUpPrefabs(StartOfRound __instance)
        {
            GameObject shipParent = GameObject.Find(Constants.SHIP_PATH);
            
            foreach (var data in prefabs)
            {
                GameObject? newObj = null;
        
                // Spawn prefab if server
                if (__instance.IsServer || __instance.IsHost)
                {
                    var prefab = GetPrefab(data.name);

                    if (prefab is null)
                    {
                        Logger.Error($"The prefab '{data.name}' was not found in the bundle!");
                        continue;
                    }
        
                    newObj = UnityEngine.Object.Instantiate(prefab);
                    
                    var networkObj = newObj.GetComponent<NetworkBehaviour>().NetworkObject;
                    networkObj.Spawn();
                    networkObj.TrySetParent(shipParent);
                }

                foreach (Transform child in shipParent.transform)
                {
                    if (child.gameObject.name != data.name + "(Clone)")
                        continue;

                    newObj = child.gameObject;
                    break;
                }
                
                if (newObj is null)
                {
                    Logger.Error($"The prefab '{data.name}' was not found in the scene!");
                    continue;
                }
                
                data.onSetup?.Invoke(newObj);
            }
        }
    }

    #endregion
}

