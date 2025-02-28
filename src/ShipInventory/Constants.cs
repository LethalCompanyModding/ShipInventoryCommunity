namespace ShipInventory;

public static class Constants
{
    // --- PATHS ---
    public const string DROP_NODE_PATH = "DropNode"; // Path to the drop node

    // --- TAGS ---
    public const string STORED_ITEMS = "shipInventoryItems"; // Key for the data of the mod
    public const string BAD_ITEM_KEYS = "shipInventoryMissingKeys"; // Key for the data of the mod

    // --- ASSETS ---
    public const string CHUTE_PREFAB = "ChutePrefab"; // Name of the prefab
    public const string MOD_ICON = "icon.png"; // Name of the mod's icon
    public const string ERROR_ITEM_ASSET = "ErrorItem"; // Name of the item
    public const string INVENTORY_BUY_TERMINAL_NODE = "InventoryBuy"; // Name of the terminal node to buy

    // --- BUNDLES ---
    public const string BUNDLE_MAIN = "ShipInventory.bundle";

    // --- LAYERS ---
    public const string LAYER_PROPS = "Props"; // Name of the layer Props
    public const string LAYER_IGNORE = "Ignore Raycast"; // Name of the layer Ignore Raycast
    public const string LAYER_INTERACTABLE = "InteractableObject"; // Name of the layer Interactable Object

    // --- TERMINAL ---
    public const int ITEMS_PER_PAGE = 10;
}
