# Changelog

## [1.2.9]() (2025-02-27)
## Changes
- Fixed log level for chute creation
- ShipInventory Bundle will be packaged outside of the file for now on.

## [1.2.8]() (2025-02-26)
## Additions
* This update is functionally the same as Ship inventory's 1.2.6 update. This is just incremented to avoid any confusions.

## [1.2.6]() (2025-02-26)
## Fixes
* Fixed how the items spawn, now spawning inside the ship correctly

## [1.2.5]() (2025-01-18)
## Fixes
* Fixed the blacklist to be case-insensitive
* Fixed items clearing when storing the chute
* Fixed items being parented to the chute
* Fixed a crash when a client tries to respawn the chute after being fired

## [1.2.4]() (2025-01-12)
## Fixes
* Fixed an issue where selecting "Understood" does nothing
* Fixed the blacklist to ignore whitespaces before and after items
* Fixed the chute not respawning after fire when it's not an upgrade
* Fixed the chute clearing itself even when 'PersistentThroughFire' was set to true

## Additions
* Added the config 'InventorySortOrder', which allows to show the inventory in four different orders 
* Added the config 'KeepRemovalAll', which allows to enable/disable the 'retrieve all' option for all players
* Added compatibility with OpenMonitors, now adding the total in the chute to the Loot monitor
* Added a support link in the README.md
* Added a license in the README.md

## Changes
* Changed the blacklist to support REGEX patterns

## [1.2.3]() (2025-01-01)

## Fixes
* Fixed a type in the english version of the configs
* Fixed typo in ItemManager.cs
* Fixed the client desync when a client leaves and rejoin

## Additions
* Added a simplified chinese version of the configs (by [CoolLKKPS](https://github.com/CoolLKKPS))
* Added a way for other mods to define how an item should convert into data (Helpers/ConvertItemHelper)
* Added a compatibility with Shopping Carts and any container item from [Custom Item Behaviour Library](https://thunderstore.io/c/lethal-company/p/WhiteSpike/Custom_Item_Behaviour_Library/)

## Changes
* Removed the update for the host, due to it being unnecessary
* Changed the belt bags to store themselves and their items instead of just themselves
* Softened the error when an item has an invalid ID (Exception => Logged Error)

## [1.2.2]() (2024-12-30)

## Fixes
* Fixed the problems related to the unlock feature

## Additions
* Added the config 'UnlockName', which defines the command to type to buy/retrieve the chute
* Added a locked screen when the chute is not present

## Changes
* Changed 'ChuteIsUnlock' to  only requires a lobby reload

## [1.2.1]() (2024-12-29)

## Fixes:
* Fixed the languages being wrongly extracted
* Fixed the chute movement when the chute is not an upgrade
* Removed the dependency of SmartItemSaving

## [1.2.0](https://github.com/WarperSan/ShipInventory/releases/tag/1.2.0) (2024-12-29)

## Fixes:
* <strong>Changed how the item system works</strong>

## Additions:
* Added a way for other mods to define if an item is allowed or not (Helpers/InteractionHelper)
* Added a proper support for the missing items. Now, missing items transform into 'Bad Item'
* Added the config 'TimeToStore', which defines how quick you can store an item
* Added the config 'InventoryCommand', which defines what is the keyword to access the ship's inventory
* Added the config 'InventoryRefreshRate', which defines how frequent the inventory refreshes
* Added the config 'InventoryUpdateCheckSilencer', which prevents the logging when the inventory is up-to-date
* Added the config 'ForceUpdateUponAdding', which forces the local inventory to get updated when storing an  item
* Added the config 'ForceUpdateUponRemoving', which forces the local inventory to get updated when retrieving an item
* Added the config 'KeepRate', which allows to define how likely each item is to be kept when wiping
* Added the config 'IsUnlock', which defines if the chute is an upgrade to buy or if it is already unlocked by default
* Added the config 'UnlockCost', which defines how expensive the upgrade is

## Changes:
* Simplified the language package (lang-en.json => langs/en.json)
* Organized configs
* Renamed SpawnDelay to TimeToRetrieve
* Made the chute an unlockable upgrade
* Made the chute movable

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