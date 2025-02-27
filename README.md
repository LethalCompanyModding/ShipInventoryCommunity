# Ship Inventory Updated
This repository is a fork of the original [ShipInventory](https://github.com/WarperSan/ShipInventory) with permission. Adds a ship object that allows players to safely store items inside the ship's inventory.

<details>
  <summary>Special Thanks to</summary>
<ul>
  <li><a href="https://github.com/minimusubi">minimusubi</a>: Rework of the terminal UI</li>
  <li><a href="https://github.com/WarperSan">WarperSan</a>: Original creator of the mod</li>
</ul>
</details>

- This mod is required on all clients for it to work.
- Highly customizable
- All accessible by one command in terminal: `ship`
- Compatible with saving and loading the game. The inventory is stored in the save file itself.
  - Note: Adding or Removing Mods that add items or remove may corrupt the save
- Has support for different languages.
  - Copy the lang-en.json file and replace values. And put the file in the same folder as the mod itself.

## Features

### Vent Chute
By holding an item to the vent chute, you can put items into the ship's inventory. You can also retrieve them using the terminal.

![Image of the vent chute](https://raw.githubusercontent.com/WarperSan/ShipInventory/master/ThunderStore/Assets/vent_holding_apparatus.png)

### Command
To access the ship's inventory, you need to type `ship` into the terminal. A window will show up, showing you every option available for the mod.

![Image of the command](https://raw.githubusercontent.com/WarperSan/ShipInventory/master/ThunderStore/Assets/ship_command.png)

# Configuration
This mod tries to give a lot of configuration and control to the user to customize how they want to. If you would like to know more

<details>
<summary>Config Settings</summary>

## General
### Language
This setting determines what language package the mod uses. Upon starting up, the mod will look for the file named `lang-CODE`, where `CODE` is the value of this parameter. This allows anyone to create their own language pack.

## Chute
### Store Permission
This setting determines who can store items into the chute. This is on top of the base item check. For example, if the setting is set to `CLIENTS_ONLY`, only clients will be able to store items in the chute.

If a player tries to store an item while not having the required permission, a special message will appear to notify them that they can't store items. 

### Only In Orbit
This setting determines when the chute can store items. If the setting is set to `true`, no one will be able to store items inside the chute while the ship is on a planet. The chute will be enabled again once the ship is back in orbit.

If a player tries to store an item while the ship is not in orbit, a special message will appear to notify them that they can't store items. 

### Time to Store
This setting determines how long a player has to hold the `interact` button in order to store an item. If players usually store items in bulk, the setting can be set to a lower value, allowing them to store items faster.

### Time to Retrieve
This setting determines how fast each item takes to be retrieved. If players usually retrieve items in bulk, the setting can be set to a lower value, allowing the process to be done way faster.

### Max Chute Capacity
This setting determines how many items can be in the chute itself at once before pausing the retrieve process. For example, if the setting is set to `5` but a player retrieves 20 items, the chute will pause after retrieving 5 items.

### Blacklist
This setting determines which items are not allowed to be stored in the chute. The mod already blocks certain items due to not working with the system, but players can also block certain items that they don't want to allow.

Here are the different things you should know about it:
- It is case insensitive (`flashlight` and `FlASHLiGht` are the same);
- It is space insensitive (spaces before and after an item are removed);
- It supports REGEX expression (`.*light` for every items that ends with `light`);
- It only checks for the `itemName` (it is the name shown in the terminal);
- Each item is separated by a `,` (`item1,item2,item3,item4`);

If a player tries to store an item that is blacklisted, a special message will appear to notify them that this item is not valid.

## Inventory
### Retrieve Permission
This setting determines who can retrieve items from the inventory. For example, if the setting is set to `CLIENTS_ONLY`, only clients will be able to retrieve items from the inventory. The host itself will not be able to retrieve items.

### Safe Once Stored
This setting determines if the inventory is cleared when all players die. This is useful if you want to use this mod as a safe, allowing you to keep some items in order to recover better from the loss.

### Persist through fire
This setting determines if the inventory is cleared when the crew gets fired. This is useful if you want to have an advantage when restarting a run, removing some of the grinding by allowing you to keep some items.

### Maximum Item Count
This setting determines how many items can be stored in the inventory. For example, if the setting is set to `20`, players will only be able to store 20 items in the inventory before needing to retrieve some items to free up space.

If a player tries to store an item while the inventory is full, a special message will appear to notify them that the inventory is already full.

### Keep Rate
This setting determines how likely each item is to be kept when the inventory is cleared. For example, if the setting is set to `50`, each item has a 50% chance to be kept or to be cleared.

## Terminal
### Inventory Command
This setting determines the command to type in order to access the inventory. Commands are something exclusive, so you might need to change the command to something else in order to be able to type it.

### Yes Please!
This setting determines if the cursor is placed on the `yes` option when retrieving an item. This is useful when you need to retrieve items in bulk, but you don't want to have to scroll down each time.

### Terminal Confirmation
This setting determines if the inventory shows a confirmation when retrieving an item. This is useful when you need to retrieve items in bulk, without worrying about the confirmation.

### Show Trademark
This setting determines if the inventory shows the trademark in the terminal. Even though the trademark is themed around Lethal Company (I could've put my own trademark), it takes unnecessary space in the terminal.

### Sort Order
This setting determines in which order the items are shown in the inventory. For example, if the setting is set to `VALUE_DESC`, the most valuable items are going to be on the first pages while the least valuable items will be at the end.

### Allow Retrieve All
This setting determines if the `retrieve all` option is shown for every player. This is useful to stop players from simply dumping every item possible, with no way to stop them. While it's still possible to retrieve items in bulk, it's slower than using the `retrieve all` option.

## Network
### Refresh Rate
This setting determines how often clients will update their copy of the inventory. 

This was implemented due to past problems with desync between clients. However, this comes at a cost of an higher WIFI cost, sending a small request every time (the cost is actually very small, almost negligible).

### Update Silencer
This setting determines if the mod logs each update in the console. The logs can be useful for developers to debug the mod.

*If you decide to remove this, you are on your own for any desync problem*

### Force when Storing
This setting determines if the client forces to update itself when the player stores an item.

### Force when Retrieving
This setting determines if the client forces to update itself when the player retrieves an item.

## Unlock
### Is Unlockable
This setting determines if the chute is an unlockable upgrade. If you enter a lobby with the setting set to `false`, the chute will be permanently unlocked for this save.

### Unlock Cost
This setting determines how expensive the upgrade is to buy. This is useful if you want to balance this mod.

### Unlock Name
This setting determines the command to type in order to buy the inventory. Commands are something exclusive, so you might need to change the command to something else in order to be able to type it.

</details>



# Credits

- "vent_chute" (https://skfb.ly/ozTrw) by jazz-the-giraffe is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).

# Support
If you enjoy the original work, consider supporting the original creator on Ko-Fi ❤️

[![kofi](https://img.shields.io/badge/kofi-%23F16061.svg?&style=for-the-badge&logo=ko-fi&logoColor=white)](https://ko-fi.com/warpersan)

# License

This project is Licensed under the [MIT](https://github.com/WarperSan/ShipInventory/blob/master/LICENSE) License.
