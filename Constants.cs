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
    
    public const string POSITIVE_ANSWER = "Yes";
    public const string NEGATIVE_ANSWER = "No";
    
    public const string PREVIOUS = "[ PREVIOUS ]";
    public const string NEXT = "[ NEXT ]";
    
    // --- CONFIGS ---
    public const string NAME_BLACKLIST = "Blacklist";
    public const string NAME_SPAWN_DELAY = "Retrieve Rate";
    public const string NAME_REQUIRES_IN_ORBIT = "In Orbit";
    public const string NAME_AS_SAFE = "Safe once stored";
    public const string NAME_STOP_AFTER = "Max Chute Capacity";
    public const string NAME_SHOW_CONFIRMATION = "Terminal Confirmation";
    public const string NAME_SHOW_NEWS = "Panel News";
    
    public const string DESCRIPTION_BLACKLIST = "List of items that are not allowed in the chute.\nThe items' name should be separated by a comma (,).";
    public const string DESCRIPTION_SPAWN_DELAY = "Time in seconds between each item spawn.";
    public const string DESCRIPTION_REQUIRE_IN_ORBIT = "Determines if the ship needs to be in orbit in order to put items inside the ship's inventory.";
    public const string DESCRIPTION_AS_SAFE = "Determines if the ship's inventory acts as a safe.\nIf set to true, this will prevent the ship's inventory from being wiped upon death.";
    public const string DESCRIPTION_STOP_AFTER = "Determines how many items can be in the chute before waiting to spawn the items.\nThe extra items will simply wait until a slot is available.";
    public const string DESCRIPTION_SHOW_CONFIRMATION = "Determines if the terminal displays a confirmation before retrieving items from the ship's inventory.";
    public const string DESCRIPTION_SHOW_NEWS = "Determines if the chute's panel should show the random joky texts that don't really mean anything.";
}