using System.Collections.Generic;
using System.Linq;
using ShipInventory.Objects;

namespace ShipInventory.Commands;

/// <summary>
/// Node used to display and recover the items stored in the ship's inventory
/// </summary>
public class InventoryNode : TerminalNode
{
    private const string STORED_ITEMS = "[storedItems]";

    public InventoryNode()
    {
        displayText = $"When handling items, utilize the drop chute\nto store them in the ship’s inventory.\n\nTo retrieve an item, simply type its name below.\nFor bulk retrieval, specify the quantity required.\n\n{STORED_ITEMS}\n";
        clearPreviousText = true;
        terminalOptions =
        [
            new CompatibleNoun()
        ];
    }

    public static string ParseText(string original)
    {
        var items = ChuteInteract.GetItems();
        
        // Set text
        string content = "";

        if (items.Any())
        {
            content += "Total: $" + items.Sum(i => i.SCRAP_VALUE) + "\n";

            // - NAME xQT ($CRD)
            List<(string, string)> lines = [];
            
            foreach (var data in items.GroupBy(d => d.ID))
            {
                var singleData = data.First();
                var item = singleData.GetItem();

                // Skip if invalid
                if (item == null)
                    continue;

                string line = $"\u2219 {item.itemName} x{data.Count()}";

                // Add average value
                if (item.isScrap)
                {
                    var avg =  data.Average(d => d.SCRAP_VALUE);
                    line += $" (AVG: ${avg:N2})";
                }
                
                lines.Add((item.itemName, line));
            }

            content = lines.Aggregate(content, (current, line) => current + line.Item2 + "\n");
        }
        else
            content += "No item  stored!";

        return original.Replace(STORED_ITEMS, content);
    }
    public static TerminalNode GetOption(Terminal terminal, string addedText)
    {
        // Update options
        var options = new List<CompatibleNoun>();
        var items = ChuteInteract.GetItems()
            .GroupBy(i => i.ID)
            .Select(i => i.First());

        // Create an option for every item
        foreach (var data in items.GroupBy(i => i.ID).Select(g => g.First()))
        {
            var option = data.CreateOption();
            
            // Skip if invalid
            if (option is null)
                continue;
            
            options.Add(option);
        }

        // Return node
        return terminal.ParseWordOverrideOptions(addedText, options.ToArray());
    }
}