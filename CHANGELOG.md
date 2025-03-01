# Changelog

<details>
<summary>1.2.11</summary>
## Fixes
- Missing graphic when depositing items should be fixed
- Fixed desync issue on multiplayer
- Fixed missing object place sound
</details>

<details>
<summary>1.2.10</summary>

## Changes
- Asset Bundle loads earlier to prevent an error with LethalConfig
- Sprite is created at runtime to prevent an error with loading the icon despite it being there.
- ShipInventory should no longer duplicate when used alongside Terminal Stuff.
</details>

<details>
<summary>1.2.9</summary>
## Changes
- Fixed log level for chute creation
- ShipInventory Bundle will be packaged outside of the file for now on.
</details>

<details>
<summary>1.2.8</summary>
## Additions
* This update is functionally the same as Ship inventory's 1.2.6 update. This is just incremented to avoid any confusions.
</details>

<details>
<summary>1.2.6</summary>
## Fixes
* Fixed how the items spawn, now spawning inside the ship correctly
</details>

<details>
<summary>1.2.5</summary>
## Fixes
* Fixed the blacklist to be case-insensitive
* Fixed items clearing when storing the chute
* Fixed items being parented to the chute
* Fixed a crash when a client tries to respawn the chute after being fired
</details>

<details>
<summary>1.2.4</summary>
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
</details>

<details>
<summary>1.2.3</summary>
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
</details>

<details>
<summary>1.2.2</summary>
## Fixes
* Fixed the problems related to the unlock feature

## Additions
* Added the config 'UnlockName', which defines the command to type to buy/retrieve the chute
* Added a locked screen when the chute is not present

## Changes
* Changed 'ChuteIsUnlock' to  only requires a lobby reload
</details>

<details>
<summary>1.2.1</summary>
## Fixes:
* Fixed the languages being wrongly extracted
* Fixed the chute movement when the chute is not an upgrade
* Removed the dependency of SmartItemSaving
</details>

<details>
<summary>1.2.0</summary>
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
</details>

<details>
<summary>1.1.8</summary>
## Fixes:
* Saving the items in the inventory now works
</details>

<details>
<summary>1.1.7</summary>
<p style="color:#AAAA00">Previous saves are not compatible (again)</p>

## Fixes:
* Changed how items are stored, using mod and item names instead of IDs

## Additions:
* Fallback item if an item is not found

</details>

<details>
<summary>1.1.6</summary>
<p style="color:#AAAA00">Previous saves are not compatible</p>

### Fixes:
* Changed how items are stored, using IDs instead of names

### Additions:
* Permission to store items in the chute
* Permission to retrieve items from the inventory

</details>

<details>
<summary>1.1.5</summary>
Not sure what changed, since dates are messed up.
</details>

<details>
<summary>1.1.4</summary>
### Fixes:
* The inventory now clears only when all players died
</details>

<details>
<summary>1.1.3</summary>
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
</details>

<details>
<summary>1.1.2</summary>
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
</details>

<details>
<summary>1.1.1</summary>
### Additions:
* Added localization support 
* Added 2 configs 
* Added that the inventory resets when fired
</details>

<details>
<summary>1.0.4</summary>
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
</details>

<details>
<summary>1.0.3</summary>
### Additions:
* Added UI from Interactive Terminal API 
* Added a blacklist for the items allowed
* Added a cooldown for the item spawn 
* Added multiple configs 
* Added a panel to see the status of the chute

### Fixes:
* Fixed a duplication glitch
</details>

<details>
<summary>1.0.2</summary>
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

</details>

<details>
<summary>1.0.1</summary>
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
</details>

<details>
<summary>1.0.0</summary>
* Mod itself :3
</details>