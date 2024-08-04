using System.Collections.Generic;
using System.Linq;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using UnityEngine;

namespace ShipInventory.Commands;

/// <summary>
/// Node used to display and recover the items stored in the ship's inventory
/// </summary>
public class InventoryNode : TerminalNode
{
    public InventoryNode()
    {
        displayText = $"When handling items, utilize the drop chute\nto store them in the ship’s inventory.\n\nTo retrieve an item, simply type its name below.\nFor bulk retrieval, specify the quantity required.\n\nTotal: ${Constants.TOTAL}\n{Constants.ITEMS}\n";
        clearPreviousText = true;
        terminalOptions =
        [
            new CompatibleNoun()
        ];
    }

    public static string ParseText(string original)
    {
        var items = ItemManager.GetItems().GroupBy(i => i.ID);

        return ItemManager.DisplayItems(original, items, ItemManager.PrintExtra.AVG);
    }
    public static TerminalNode GetOption(Terminal terminal, string addedText)
    {
        // Update options
        var options = new List<CompatibleNoun>();

        // Create an option for every item
        foreach (var group in ItemManager.GetItems().GroupBy(i => i.ID))
        {
            var option = group.First().CreateOption();
            
            // Skip if invalid
            if (option is null)
                continue;

            if (option.result is SuccessNode successNode)
                successNode.count = Mathf.Min(terminal.playerDefinedAmount, group.Count());
            
            options.Add(option);
        }

        // Return node
        return terminal.ParseWordOverrideOptions(addedText, options.ToArray());
    }
}