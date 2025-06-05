using HarmonyLib;
using UnityEngine;
using Logger = ShipInventoryUpdated.Helpers.Logger;

namespace ShipInventoryUpdated.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class Terminal_Patches
{
    [HarmonyPatch(nameof(Terminal.Awake)), HarmonyPrefix]
    private static void Awake_Prefix(Terminal __instance)
    {
        const string name = "ship inventory";
        
        RegisterUnlockable(name);
        RegisterTerminal(__instance, name);
    }

    private static void RegisterUnlockable(string name)
    {
        if (StartOfRound.Instance == null)
        {
            Logger.Error($"Tried to find '{nameof(StartOfRound.Instance)}', but it was not defined.");
            return;
        }
        
        if (ShipInventoryUpdated.CHUTE_BUY_NODE == null)
        {
            Logger.Error($"Tried to add '{nameof(ShipInventoryUpdated.CHUTE_BUY_NODE)}' to the terminal, but it was not defined.");
            return;
        }
        
        if (ShipInventoryUpdated.CHUTE_CONFIRM_NODE == null)
        {
            Logger.Error($"Tried to add '{nameof(ShipInventoryUpdated.CHUTE_CONFIRM_NODE)}' to the terminal, but it was not defined.");
            return;
        }
        
        var unlockables = StartOfRound.Instance.unlockablesList.unlockables;
        var unlockableID = unlockables.Count;
        
        var unlockable = new UnlockableItem
        {
            unlockableName = name,
            prefabObject = ShipInventoryUpdated.CHUTE_PREFAB,
            unlockableType = 1,
            shopSelectionNode = null,
            alwaysInStock = true,
            IsPlaceable = true,
            canBeStored = true,
            maxNumber = 1,
            spawnPrefab = true
        };
        
        unlockables.Add(unlockable);

        var placeable = unlockable.prefabObject?.GetComponentInChildren<PlaceableShipObject>();
        
        if (placeable != null)
            placeable.unlockableID = unlockableID;

        ShipInventoryUpdated.CHUTE_BUY_NODE.shipUnlockableID = unlockableID;
        ShipInventoryUpdated.CHUTE_CONFIRM_NODE.shipUnlockableID = unlockableID;
    }

    private static void RegisterTerminal(Terminal __instance, string name)
    {
        const string buyWord = "buy";
        TerminalKeyword? buyNode = null;
        
        var unlockKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
        unlockKeyword.name = unlockKeyword.word = name;
        
        __instance.terminalNodes.allKeywords = __instance.terminalNodes.allKeywords.AddToArray(unlockKeyword);

        foreach (var keyword in __instance.terminalNodes.allKeywords)
        {
            if (keyword.word != buyWord)
                continue;

            buyNode = keyword;
            break;
        }

        if (buyNode == null)
        {
            Logger.Error($"Tried to add the noun to the keyword '{buyWord}', but it was not found.");
            return;
        }

        buyNode.compatibleNouns = buyNode.compatibleNouns.AddToArray(new CompatibleNoun
        {
            noun = unlockKeyword,
            result = ShipInventoryUpdated.CHUTE_BUY_NODE
        });
        
        unlockKeyword.defaultVerb = buyNode;
    }
}