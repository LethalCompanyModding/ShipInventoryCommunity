using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using ShipInventory.Commands;
using ShipInventory.Objects;
using UnityEngine;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(Terminal))]
internal class Terminal_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.Awake))]
    private static void AddKeywords(Terminal __instance)
    {
        // INVENTORY
        var inventory = ScriptableObject.CreateInstance<TerminalKeyword>();
        inventory.word = "inventory";
        inventory.specialKeywordResult = ScriptableObject.CreateInstance<InventoryNode>();
        
        // RETRIEVE
        var retrieve = ScriptableObject.CreateInstance<TerminalKeyword>();
        retrieve.word = "retrieve";
        retrieve.specialKeywordResult = ScriptableObject.CreateInstance<RetrieveNode>();
        
        // Add all keywords
        __instance.terminalNodes.allKeywords = __instance.terminalNodes.allKeywords.AddRangeToArray([
            inventory,
            retrieve
        ]);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.TextPostProcess))]
    private static void ParseCustomText(Terminal __instance, TerminalNode node, ref string __result)
    {
        // Parse text
        __result = node switch
        {
            InventoryNode => InventoryNode.ParseText(__result),
            RetrieveNode retrieveNode => retrieveNode.ParseText(__result),
            SuccessNode successNode => successNode.ParseText(__result),
            _ => __result
        };
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.ParsePlayerSentence))]
    private static void CustomParsePlayerSentence(Terminal __instance, ref TerminalNode __result)
    {
        // Get text
        string added = __instance.screenText.text[^__instance.textAdded..];
        added = __instance.RemovePunctuation(added);
        
        // Get player defined value
        int.TryParse(Regex.Match(added, "\\d+").Value, out int playerDefinedAmount);
        
        // Get keyword
        var keyword = __instance.CheckForExactSentences(added.Split(" ")[0]);
        
        TerminalNode? attempt = null;

        // If previous node is InventoryNode
        if (__instance.currentNode is InventoryNode)
            attempt = InventoryNode.GetOption(__instance, added);

        // If special keyword is RetrieveNode
        if (keyword?.specialKeywordResult is RetrieveNode retrieveNode)
        {
            retrieveNode.TryRetrieve(playerDefinedAmount);
            attempt = retrieveNode;
        }

        if (attempt != null)
            __result = attempt;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.RunTerminalEvents))]
    private static void ExecuteCustomEvents(Terminal __instance, TerminalNode node)
    {
        if (string.IsNullOrWhiteSpace(node.terminalEvent))
            return;

        if (node.terminalEvent != Constants.VENT_SPAWN)
            return;
        
        if (node is SuccessNode { selectedItem: not null } successNode)
            ChuteInteract.Instance.SpawnItemServerRpc(successNode.selectedItem.Value, __instance.playerDefinedAmount);
        
        if (node is RetrieveNode retrieveNode)
        {
            foreach (var group in retrieveNode.GetItems() ?? [])
            {
                ChuteInteract.Instance.SpawnItemServerRpc(
                    group.First(),
                    group.Count()
                );
            }
        }
    }
}