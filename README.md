# Ship Inventory
Adds a ship object that allows players to safely store items inside the ship's inventory.

There are 2 reasons for why I made this mod:
1. One of my friends kept throwing scrap off the ship, making us lose
2. I get lag spikes when around the ship (which I assume is because of the items)

## Vent Chute
By holding an item to the vent chute, you can put items into the ship's inventory. You can also retrieve them using the terminal.

![Image of the vent chute](https://raw.githubusercontent.com/WarperSan/ShipInventory/master/ThunderStore/Assets/vent_holding_apparatus.png)

## Command
To access the ship's inventory, you need to type `ship` into the terminal. A window will show up, showing you every option available for the mod.

![Image of the command](https://raw.githubusercontent.com/WarperSan/ShipInventory/master/ThunderStore/Assets/ship_command.png)

## Configs
The mod gives a lot of variety in terms of customizability. This is a list of all the configs available. Note  that the description are different here and in the mod.

| Name                  | Description                                                                                        |
|-----------------------|----------------------------------------------------------------------------------------------------|
| Blacklist             | List of items that are not allowed in the chute. This is a restriction on top of  the current one. |
| Retrieve rate         | Time between each spawn (to prevent freezes/crashes)                                               |
| Only In Orbit         | Prevents to put items inside (in case you find it OP)                                              |
| Safe once stored      | If checked, the inventory won't be wiped out when the crew wipes out                               |
| Max Capacity          | Limit of items in the chute (to prevent excessive lag)                                             |
| Max Item Count        | Limits the number of items inside the inventory                                                    |
| Terminal Confirmation | Toggles the confirmation before retrieving items                                                   |
| No Steam ID           | Replaces the ID shown by a randomized ID                                                           |

## Save/Load
When the host saves or loads a save file, the mod will add the stored items inside the file to read them when loading. 

*Note that it uses the default method of saving items*

## Compatibility
The mod is compatible with mods that fetch the total value of the ship (like General Improvements or Ship Loot). However, it is not compatible with mods that allow you to sell scrap via the terminal. The game considers the inventory as a single item.