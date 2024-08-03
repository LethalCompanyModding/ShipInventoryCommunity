# Ship Inventory
Adds a ship object that allows players to safely store items inside the ship's inventory.

## Vent Chute
By holding an item to the vent chute, you can put items into the ship's inventory. You can also retrieve them using the terminal.

![Image of the vent chute](https://github.com/WarperSan/ShipInventory/blob/master/ThunderStore/vent_holding_apparatus.png)

## Commands
The players have access to two commands:
- Inventory
- Retrieve

### Inventory
By typing 'inventory' in the terminal, the terminal will show you all the items that are currently stored in the ship's inventory.

![Image of the inventory](https://github.com/WarperSan/ShipInventory/blob/master/ThunderStore/inventory.png)

Here, you can see all the items and their average value. If you want to retrieve an item, you can type the name of the item (e.g.: 'app' for the Apparatus). Optionally, you can also define how many items you want by adding a number.

### Retrieve
By typing 'retrieve' followed by a number, the terminal will retrieve items that add up to the given number.

![Image of the retrieve](https://github.com/WarperSan/ShipInventory/blob/master/ThunderStore/retrieve.png)

*Note that the terminal will try to find the combination of the fewer items with the least overflow.*

## Save/Load
When the host saves or loads a save file, the mod will add the stored items inside the file to read them when loading.

## Wipe out
The ship's inventory is not a safe. If the crew wipes out, the inventory will also wipe out.