namespace ShipInventory;

public static class Constants
{
    // --- PATHS ---
    public const string SHIP_PATH = "Environment/HangarShip"; // Path for the ship

    // --- TAGS ---
    public const string STORED_ITEMS = "shipInventoryItems"; // Key for the data of the mod
    
    // --- ASSETS ---
    public const string VENT_PREFAB = "VentChute"; // Name of the prefab
    public const string PANEL_PREFAB = "ChutePanel"; // Name of the prefab
    public const string MOD_ICON = "icon"; // Name of the mod's icon
    public const string BUNDLE = "ShipInventory.Resources.si-bundle"; // Name of the bundle
    
    // --- LAYERS ---
    public const string LAYER_PROPS = "Props"; // Name of the layer Props
    public const string LAYER_IGNORE = "Ignore Raycast"; // Name of the layer Ignore Raycast
    public const string LAYER_INTERACTABLE = "InteractableObject"; // Name of the layer Interactable Object
    
    // --- STRINGS ---
    public const string UNKNOWN = "???";
    public const string NOT_HOLDING_ITEM = "[Nothing to store]"; // Tip when the player has no item
    public const string ITEM_NOT_ALLOWED = "[Item not allowed]"; // Tip when the player has an illegal item
    public const string NOT_IN_ORBIT = "[Ship must be in orbit]"; // Tip when the ship is not in orbit
    public const string INVENTORY_FULL = "[Inventory full]"; // Tip when the ship's inventory is full
    public const string TEXT_RANDOM_RETRIEVE = "You are about to retrieve '{0}' from the ship's inventory.";
    public const string TEXT_ALL_RETRIEVE = "You are about to retrieve everything from the ship's inventory.";
    public const string TEXT_SINGLE_RETRIEVE = "You are about to retrieve '{0}' from the ship's inventory.";
    public const string TEXT_TYPE_RETRIEVE = "You are about to retrieve {0} instances of '{1}' from the ship's inventory.";
    
    // --- TERMINAL ---
    public const int ITEMS_PER_PAGE = 10;
    
    public const string SINGLE_RETRIEVE = "Retrieve single";
    public const string TYPE_RETRIEVE = "Retrieve type";
    public const string RANDOM_RETRIEVE = "Retrieve random";
    public const string ALL_RETRIEVE = "Retrieve all";
    public const string SHIP_INFO = "Print Status";
    
    public const string POSITIVE_ANSWER = "Yes";
    public const string NEGATIVE_ANSWER = "No";
    
    public const string PREVIOUS = "[ PREVIOUS ]";
    public const string NEXT = "[ NEXT ]";
    
    // --- CONFIGS ---
    public const string TERMINAL_SECTION = "Terminal";
    public const string INVENTORY_SECTION = "Inventory";
    public const string CHUTE_SECTION = "Chute";
    
    #region Chute

    public const string NAME_BLACKLIST = "Blacklist";
    public const string DESCRIPTION_BLACKLIST = "List of items that are not allowed in the chute.\n\nThe items' name should be separated by a comma (,).\nYou can edit the config directly in the file for a better experience.";

    public const string NAME_SPAWN_DELAY = "Retrieve Rate";
    public const string DESCRIPTION_SPAWN_DELAY = "Time in seconds between each item spawn.";
    
    public const string NAME_REQUIRES_IN_ORBIT = "Only In Orbit";
    public const string DESCRIPTION_REQUIRE_IN_ORBIT = "Prevents players from putting items inside the chute when the ship is on a planet.";
    
    public const string NAME_STOP_AFTER = "Max Chute Capacity";
    public const string DESCRIPTION_STOP_AFTER = "Limits the amount of items that can be spawned inside the chute.\n\nThis is to prevent further lag when retrieve too many items.";

    #endregion
    
    #region Inventory

    public const string NAME_AS_SAFE = "Safe once stored";
    public const string DESCRIPTION_AS_SAFE = "Prevents the ship's inventory from clearing when all players die.";

    public const string NAME_MAX_ITEM_COUNT = "Maximum Item Count";
    public const string DESCRIPTION_MAX_ITEM_COUNT = "Limits the amount of items that can be inside the ship's inventory.\n\nIf the ship's inventory already has too many items, it won't empty the excess.";

    #endregion
    
    #region Terminal

    public const string NAME_SHOW_CONFIRMATION = "Terminal Confirmation";
    public const string DESCRIPTION_SHOW_CONFIRMATION = "Removes the confirmation when retrieving items from the ship's inventory.";

    public const string NAME_NO_STEAM_ID = "Show Steam ID";
    public const string DESCRIPTION_NO_STEAM_ID = "Replaces the ID shown on the ship's inventory meny by a random ID.";

    #endregion
}
