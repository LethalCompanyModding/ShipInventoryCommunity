using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using UnityEngine;

// ReSharper disable PossibleMultipleEnumeration

namespace ShipInventory.Commands;

/// <summary>
/// Node used to retrieve items easily from the inventory
/// </summary>
public class RetrieveNode : TerminalNode
{
    private IEnumerable<ItemData> items = [];

    public IEnumerable<IGrouping<int, ItemData>> GetItems() 
        => items.GroupBy(i => i.ID).OrderBy(i => i.First().GetItem()?.itemName);

    public RetrieveNode()
    {
        Default();
        clearPreviousText = true;
        terminalOptions =
        [
            new CompatibleNoun()
        ];
        terminalEvent = Constants.VENT_SPAWN;
    }

    public string ParseText(string original)
    {
        original = ItemManager.DisplayItems(original, GetItems(), ItemManager.PrintExtra.SUM);
        
        // RESET
        Default();
        
        return original;
    }

    public void Default()
    {
        displayText = "The Company does not support his feature.\nHere are the valid parameters:\n- ALL\n- RANDOM/RDM\n\n";
        items = [];
    }

    #region Retrieve Options

    public void RetrieveRandom()
    {
        displayText = $"The Company selected a random set of items:\n{Constants.ITEMS}\nThey total to ${Constants.TOTAL}.\n\n";
        var _items = ItemManager.GetItems().ToList();

        for (int i = 0; i < Random.Range(1, _items.Count); i++)
        {
            _items.RemoveAt(Random.Range(0, _items.Count));
        }

        items = _items;
    }

    public void RetrieveAll()
    {
        displayText = $"The Company selected all the items:\n{Constants.ITEMS}\nThey total to ${Constants.TOTAL}.\n\n";
        items = ItemManager.GetItems();
    }

    #endregion
}