using System.Linq;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Commands;

/// <summary>
/// Node used to display the success of a recovery
/// </summary>
public class SuccessNode : TerminalNode
{
    private const string RECOVERED = "[recoveredItem]";
    public ItemData selectedItem;
    public int count;
    public SuccessNode()
    {
        clearPreviousText = true;
        displayText = $"You just recovered {Constants.COUNT} {RECOVERED} from the ship's inventory.\n\n";
        terminalEvent = Constants.VENT_SPAWN;
    }

    public string ParseText(string original)
    {
        original = original.Replace(RECOVERED, selectedItem.GetItem()?.itemName);
        original = original.Replace(Constants.COUNT, count.ToString());

        return original;
    }
}