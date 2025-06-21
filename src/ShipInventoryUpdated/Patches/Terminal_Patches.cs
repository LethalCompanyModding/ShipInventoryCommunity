using System.Collections.Generic;
using HarmonyLib;
using ShipInventoryUpdated.Configurations;
using ShipInventoryUpdated.Helpers;
using UnityEngine;
using Logger = ShipInventoryUpdated.Helpers.Logger;

namespace ShipInventoryUpdated.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class Terminal_Patches
{
    private static UnlockableItem? registeredUnlockable;
    private static TerminalKeyword? registeredKeyword;
    
    [HarmonyPatch(nameof(Terminal.Awake)), HarmonyPrefix]
    private static void Awake_Prefix(Terminal __instance)
    {
        var entry = Configuration.Instance?.Unlock.UnlockName;

        if (entry == null)
        {
            Logger.Error($"Tried to access the configuration for '{nameof(UnlockConfig.UnlockName)}', but it was not defined.");
            return;
        }

        RegisterUnlockable(entry.Value);
        RegisterKeyword(__instance, entry.Value);
    }

    private static void RegisterUnlockable(string command)
    {
        if (registeredUnlockable != null)
            return;
        
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
            unlockableName = command,
            prefabObject = ShipInventoryUpdated.CHUTE_PREFAB,
            unlockableType = 1,
            shopSelectionNode = null,
            alwaysInStock = true,
            IsPlaceable = true,
            canBeStored = true,
            maxNumber = 1,
            spawnPrefab = true
        };

        registeredUnlockable = unlockable;
        
        unlockables.Add(unlockable);

        var placeable = unlockable.prefabObject?.GetComponentInChildren<PlaceableShipObject>();
        
        if (placeable != null)
            placeable.unlockableID = unlockableID;

        ShipInventoryUpdated.CHUTE_BUY_NODE.shipUnlockableID = unlockableID;
        ShipInventoryUpdated.CHUTE_BUY_NODE.displayText = Localization.Get("terminal.buy.main");

        ShipInventoryUpdated.CHUTE_CONFIRM_NODE.shipUnlockableID = unlockableID;
        ShipInventoryUpdated.CHUTE_CONFIRM_NODE.displayText = Localization.Get("terminal.buy.confirm", new Dictionary<string, string>
        {
            ["command"] = command
        });
    }

    private static void RegisterKeyword(Terminal __instance, string command)
    {
        if (registeredKeyword != null)
            return;

        const string buyWord = "buy";
        TerminalKeyword? buyNode = null;
        
        var unlockKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
        unlockKeyword.name = unlockKeyword.word = command;

        registeredKeyword = unlockKeyword;
        
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

    public static void AssignNewCommand(string command)
    {
        if (registeredUnlockable != null)
            registeredUnlockable.unlockableName = command;

        if (registeredKeyword != null)
            registeredKeyword.name = registeredKeyword.word = command;

        if (ShipInventoryUpdated.CHUTE_CONFIRM_NODE != null)
        {
            ShipInventoryUpdated.CHUTE_CONFIRM_NODE.displayText = Localization.Get("terminal.buy.confirm", new Dictionary<string, string>
            {
                ["command"] = command
            });
        }
    }

    public static void AssignNewCost(int cost)
    {
        if (ShipInventoryUpdated.CHUTE_BUY_NODE != null)
            ShipInventoryUpdated.CHUTE_BUY_NODE.itemCost = cost;
        
        if (ShipInventoryUpdated.CHUTE_CONFIRM_NODE != null)
            ShipInventoryUpdated.CHUTE_CONFIRM_NODE.itemCost = cost;
    }
}