using ShipInventory.Objects;

namespace ShipInventory.Commands;

/// <summary>
/// Node used to display the success of a recovery
/// </summary>
public class SuccessNode : TerminalNode
{
    private const string RECOVERED = "[recoveredItem]";
    public ItemData? selectedItem;
    public SuccessNode()
    {
        clearPreviousText = true;
        displayText = $"You just recovered [variableAmount] {RECOVERED} from the ship's inventory.\n\n";
        terminalEvent = "chuteSpawn";
    }

    public string ParseText(string original) => !original.Contains(RECOVERED) 
        ? original 
        : original.Replace(RECOVERED, selectedItem?.GetItem()?.itemName);
}