# Changelog

## [1.1.8](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.8) (2024-12-09)

## Fixes:
* Saving the items in the inventory now works

## [1.1.7](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.7) (2024-12-08)
<p style="color:#AAAA00">Previous saves are not compatible (again)</p>

## Fixes:
* Changed how items are stored, using mod and item names instead of IDs

## Additions:
* Fallback item if an item is not found

## [1.1.6](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.6) (2024-11-25)
<p style="color:#AAAA00">Previous saves are not compatible</p>

### Fixes:
* Changed how items are stored, using IDs instead of names

### Additions:
* Permission to store items in the chute
* Permission to retrieve items from the inventory

## [1.1.5](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.5) (2024-10-24)
Not sure what changed, since dates are messed up.

## [1.1.4](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.4) (2024-09-28)
### Fixes:
* The inventory now clears only when all players died

## [1.1.3](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.3) (2024-09-28)
### Additions:
* Added a new config

### Changes:
* Use InteractiveTerminalAPI paging system
* Use InteractiveTerminalAPI exit key to go back
* Always return the user to the last menu they were in
* Shows the "copyright" on every screen
* Changed some texts to be more polite
* Changed the default value for YesPlease

### Fixes:
* Fixed the method ItemManager.GetInstances

## [1.1.2](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.2) (2024-09-27)
### Additions:
* Added soft compatibility with LethalConfig
* Added a new config (YesPlease)

### Changes:
* Replaced the Steam ID by the username 
* Removed the ability to change the language used through LethalConfig 
* Removed the VentProp.cs component from the chute, making it not a grabbable

### Fixes:
* Fixed how the items are chosen to be allowed
* Fixed bugs with ShipApplication.cs
* Fixed how items spawn from the chute

## [1.1.1](https://github.com/WarperSan/ShipInventory/releases/tag/1.1.1) (2024-09-08)
### Additions:
* Added localization support 
* Added 2 configs 
* Added that the inventory resets when fired

## [1.0.4](https://github.com/WarperSan/ShipInventory/releases/tag/1.0.4) (2024-08-25)
### Additions:
* Added cycling idle information (total, amount)
* Added method in ItemManager that returns the sum of the items stored 
* Added 6 new configs 
* Added a proper icon

### Changes:
* Organized the configs by sections

## Fixes:  
* Fixed the unlockable bug(?)
* Fixed a bug with the blacklist not updating upon joining

## [1.0.3](https://github.com/WarperSan/ShipInventory/releases/tag/1.0.3) (2024-08-16)
### Additions:
* Added UI from Interactive Terminal API 
* Added a blacklist for the items allowed
* Added a cooldown for the item spawn 
* Added multiple configs 
* Added a panel to see the status of the chute

### Fixes:
* Fixed a duplication glitch


## [1.0.2](https://github.com/WarperSan/ShipInventory/releases/tag/1.0.2) (2024-08-8)
### Additions:
* Added the command 'retrieve all' 
* Added the command 'retrieve random' 
* Added a separate class to manage the items 
* Added a blacklist for the items allowed inside

### Changes:
* The chute removes the radar icon of the items put inside it 
* The chute removes its own radar icon 
* How the terminal displays the different texts 
* Removed the command 'retrieve [amount]'
* Cleaned the class ItemData 
* Cleaned the methods in Terminal_Patches

### Fixes:
* The chute doesn't save when the other items save 
* Fixed the random clearing of the chute (only clears when all players died)
* Fixed an error inside the player request 
* The chute now doesn't count towards the day quota 
* The chute loads empty when no items were saved in the save file

## [1.0.1](https://github.com/WarperSan/ShipInventory/releases/tag/1.0.1) (2024-08-07)
### Additions:
* Unity Assets for this mod 
* Clearing the inventory upon a loss 
* License to the mod (oops)

### Changes:
* Now compatible with other scanning mods 
* When the ship's inventory gets cleared 
* Removed redundant code 
* Removed the target check for an item request

### Fixes:
* Fixed typos 
* Fixed problems related to the object Chute 
* Fixed the spawn of items from the chute 
* Fixed the scan node for the chute


## [1.0.0](https://github.com/WarperSan/ShipInventory/releases/tag/1.0.0)  (2024-08-07)
* Mod itself :3