using System.Collections.Generic;
using System.Linq;
using ShipInventory.Objects;

// ReSharper disable PossibleMultipleEnumeration

namespace ShipInventory.Commands;

public class RetrieveNode : TerminalNode
{
    private const string TARGET = "[target]";
    private const string ITEMS = "[items]";
    private const string TOTAL = "[total]";
        
    private IEnumerable<ItemData>? items;
    private int target;

    public IEnumerable<IGrouping<int, ItemData>>? GetItems() 
        => items?.GroupBy(i => i.ID).OrderBy(i => i.First().GetItem()?.itemName);

    public RetrieveNode()
    {
        displayText = $"For ${TARGET}, we found this set of items:\n{ITEMS}\n\nThey total to ${TOTAL}.\n\n";
        clearPreviousText = true;
        terminalOptions =
        [
            new CompatibleNoun()
        ];
        terminalEvent = "chuteSpawn";
    }

    public string ParseText(string original)
    {
        var _target = target.ToString();
        var total = (items?.Sum(i => i.SCRAP_VALUE) ?? 0).ToString();
        var _items = "";

        foreach (var group in GetItems() ?? [])
        {
            var item = group.First().GetItem();
            
            if (item == null)
                continue;
            
            _items = $"\u2219 {item.itemName} x{group.Count()}";

            // Add average value
            var avg =  group.Average(d => d.SCRAP_VALUE);
            _items += $" (AVG: ${avg:N2})";
        }
        
        original = original.Replace(TOTAL, total);
        original = original.Replace(TARGET, _target);
        original = original.Replace(ITEMS, string.IsNullOrEmpty(_items) ? "No items found" : _items);
        
        return original;
    }

    public void TryRetrieve(int _target)
    {
        target = _target;
        items = GetClosest(
            ChuteInteract.GetItems(),
            _target
        );
    }
    
    private static IEnumerable<ItemData> GetClosest(IEnumerable<ItemData> items, int target)
    {
        // If target invalid, skip
        if (target <= 0)
            return [];
        
        var validItems = items
            .Where(i => i.SCRAP_VALUE > 0) // Keep items with scrap value
            .OrderBy(i => i.SCRAP_VALUE) // Sort items by scrap value
            .ToArray();
        
        var solution = GetSubsets(validItems).Select(n => new
        {
            ITEMS = n,
            TOTAL = n.Sum(i => i.SCRAP_VALUE)
        })
        .Where(n => n.TOTAL >= target) // Select valid solutions
        .OrderBy(n => n.TOTAL) // Order solutions by excess
        .ThenBy(n => n.ITEMS.Count()) // Favor solutions with fewer objects
        .FirstOrDefault();
        
        return solution?.ITEMS ?? [];
    }
    private static IEnumerable<IEnumerable<ItemData>> GetSubsets(ItemData[] items) => items.Length switch
    {
        0 => [],
        1 => new[] { [], items.Take(1) },
        _ => GetSubsets(items[1..]).SelectMany(y => new[] { y, y.Append(items[0]) })
    };
}