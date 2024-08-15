namespace ShipInventory;

public static class Constants
{
    // --- PATHS ---
    public const string SHIP_PATH = "Environment/HangarShip"; // Path for the ship

    // --- TAGS ---
    public const string STORED_ITEMS = "shipInventoryItems"; // Key for the data of the mod
    public const string VENT_SPAWN = "ventSpawn"; // Name of the event to spawn from the chute
    public const string ITEMS = "[items]";
    public const string TOTAL = "[total]";
    public const string COUNT = "[count]";
    
    // --- ASSETS ---
    public const string VENT_PREFAB = "VentChute"; // Name of the prefab
    public const string BUNDLE = "ShipInventory.Resources.si-bundle"; // Name of the bundle
    
    // --- LAYERS ---
    public const string LAYER_PROPS = "Props"; // Name of the layer Props
    public const string LAYER_IGNORE = "Ignore Raycast"; // Name of the layer Ignore Raycast
    public const string LAYER_INTERACTABLE = "InteractableObject"; // Name of the layer Interactable Object
    
    // --- STRINGS ---
    public const string UNKNOWN = "???";
    public const string NOT_HOLDING_ITEM = "[Nothing to store]"; // Tip when the player has no item
    public const string ITEM_NOT_ALLOWED = "[Item not allowed]"; // Tip when the player has an illegal item
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
}