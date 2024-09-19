﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class GameNetworkManager_Patches
{
    /// <summary>
    /// Removes the chute from the selection of items to save
    /// </summary>
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(GameNetworkManager.SaveItemsInShip))]
    private static IEnumerable<CodeInstruction> RemoveChuteFromSelection(
        MethodBase original, IEnumerable<CodeInstruction> instructions)
    {
        var enumerator = instructions.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var instruction = enumerator.Current;
            
            // If instruction invalid, skip
            if (instruction == null)
                break;

            yield return instruction;
            
            // Skip if not array stored
            if (instruction.opcode != OpCodes.Stloc_0)
                continue;

            // Add instructor after storing all objects
            yield return new CodeInstruction(OpCodes.Ldloca_S, System.Convert.ToByte(0));
            yield return new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(VentProp), nameof(VentProp.RemoveChute))
            );
        }
        enumerator.Dispose();
    }
    
    /// <summary>
    /// Saves the inventory into the file
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameNetworkManager.SaveItemsInShip))]
    private static void SaveChuteItems(GameNetworkManager __instance)
    {
        // Delete items
        ES3.DeleteKey("shipGrabbableItemIDs", __instance.currentSaveFileName);
        ES3.DeleteKey("shipGrabbableItemPos", __instance.currentSaveFileName);
        ES3.DeleteKey("shipScrapValues", __instance.currentSaveFileName);
        ES3.DeleteKey("shipItemSaveData", __instance.currentSaveFileName);
        
        Logger.Debug("Saving chute items...");

        var items = ItemManager.GetItems();

        // Save items if necessary
        if (items.Any())
            ES3.Save(Constants.STORED_ITEMS, items.ToArray(), __instance.currentSaveFileName);
        else
            ES3.DeleteKey(Constants.STORED_ITEMS, __instance.currentSaveFileName);
        
        Logger.Debug("Chute items saved!");
    }
}