namespace ShipInventory;

public static class Constants
{
    // --- PATHS ---
    public const string SHIP_PATH = "Environment/HangarShip"; // Path for the ship

    // --- TAGS ---
    public const string STORED_ITEMS = "shipInventoryItems"; // Key for the data of the mod
    public const string VENT_SPAWN = "ventSpawn"; // Name of the event to spawn from the chute
    public const string VANILLA_ITEM_IDS = "shipGrabbableItemIDs"; // Key for the data of the items' ids
    public const string VANILLA_ITEM_POS = "shipGrabbableItemPos"; // Key for the data of the items' positions
    public const string VANILLA_ITEM_VALUES = "shipScrapValues"; // Key for the data of the items' values
    public const string VANILLA_ITEM_DATA = "shipItemSaveData"; // Key for the data of the items' data

    // --- ASSETS ---
    public const string VENT_PREFAB = "VentChute"; // Name of the prefab
    public const string BUNDLE = "ShipInventory.Resources.si-bundle"; // Name of the bundle
    
    // --- LAYERS ---
    public const string LAYER_PROPS = "Props"; // Name of the layer Props
    public const string LAYER_IGNORE = "Ignore Raycast"; // Name of the layer Ignore Raycast
    public const string LAYER_INTERACTABLE = "InteractableObject"; // Name of the layer Interactable Object
}