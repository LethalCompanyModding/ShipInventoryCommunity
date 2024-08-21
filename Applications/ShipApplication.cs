using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Applications;

public class ShipApplication : InteractiveTerminalApplication
{
    public override void Initialization() => Main();

    private void Main()
    {
        var player = StartOfRound.Instance.localPlayerController;

        string playerStatus = new StringBuilder()
            .Append(HUDManager.Instance.playerLevels[player.playerLevelNumber].levelName)
            .Append(" #")
            .Append(player.playerSteamId)
            .ToString();

        var optionMenu = new CursorMenu
        {
            cursorIndex = 0,
            elements =
            [
                new CursorElement
                {
                    Name = Constants.SINGLE_RETRIEVE,
                    Active = _ => ItemManager.GetItems().Any(),
                    Action = () => RetrieveSingle(0)
                },
                new CursorElement
                {
                    Name = Constants.TYPE_RETRIEVE,
                    Active = _ => ItemManager.GetItems().Any(),
                    Action = () => RetrieveType(0)
                },
                new CursorElement
                {
                    Name = Constants.RANDOM_RETRIEVE,
                    Active = _ => ItemManager.GetItems().Any(),
                    Action = RetrieveRandom
                },
                new CursorElement
                {
                    Name = Constants.ALL_RETRIEVE,
                    Active = _ => ItemManager.GetItems().Any(),
                    Action = RetrieveAll
                }
            ]
        };

        var screen = new BoxedScreen {
            Title = "Ship's console",
            elements =
            [
                new TextElement
                {
                    Text = new StringBuilder().Append("Welcome aboard, ").Append(playerStatus).ToString()
                },
                new TextElement { Text = " " },
                new TextElement
                {
                    Text = "Please select the desired action to execute from the provided list:"
                },
                new TextElement { Text = " " },
                optionMenu,
                new TextElement { Text = " " },
                new TextElement
                {
                    Text = "<color=#666666>       [ PROPERTY OF Halden Eletronics © ]</color>"
                }
            ]
        };
        SwitchScreen(screen, optionMenu, true);
    }
    private void Confirm(string message, Action? callback)
    {
        // If skip confirmation, skip
        if (!ShipInventory.Config.ShowConfirmation.Value)
        {
            callback?.Invoke();
            return;
        }
            
        var optionMenu = new CursorMenu
        {
            cursorIndex = 0,
            elements =
            [
                new CursorElement
                {
                    Name = Constants.NEGATIVE_ANSWER,
                    Action = Main
                },
                new CursorElement
                {
                    Name = Constants.POSITIVE_ANSWER,
                    Action = () => callback?.Invoke()
                }
            ]
        };

        var screen = new BoxedScreen {
            Title = "Confirmation",
            elements =
            [
                new TextElement
                {
                    Text = message
                },
                new TextElement { Text = " " },
                new TextElement
                {
                    Text = "Do you really want to proceed?"
                },
                new TextElement { Text = " " },
                optionMenu
            ]
        };
        SwitchScreen(screen, optionMenu, true);
    }

    private void RetrieveRandom()
    {
        // Random object
        var items = ItemManager.GetItems();

        object randomObj = items.ElementAt(UnityEngine.Random.Range(0, items.Count()));
        
        // Generate message
        string message = Constants.TEXT_RANDOM_RETRIEVE;

        string name = randomObj switch
        {
            ItemData data => data.GetItem()?.itemName,
            EnemyType enemyType => enemyType.enemyName,
            _ => null
        } ?? Constants.UNKNOWN;

        message = string.Format(message, name);
        
        Confirm(message, () =>
        {
            // Spawn random
            if (randomObj is ItemData data)
                ChuteInteract.Instance?.SpawnItemServerRpc(data);

            UnityEngine.Object.Destroy(InteractiveTerminalManager.Instance);
        });
    }
    private void RetrieveAll()
    {
        Confirm(Constants.TEXT_ALL_RETRIEVE, () =>
        {
            foreach (var group in ItemManager.GetItems().GroupBy(i => i.ID))
            {
                ChuteInteract.Instance?.SpawnItemServerRpc(
                    group.First(),
                    group.Count()
                );
            }
            
            UnityEngine.Object.Destroy(InteractiveTerminalManager.Instance);
        });
    }
    private void RetrieveSingle(int index)
    {
        var items = ItemManager.GetItems();
        var elements = new List<CursorElement>();
        
        // Add previous button
        if (index > 0)
        {
            elements.Add(new CursorElement
            {
                Name = Constants.PREVIOUS,
                Action = () => RetrieveSingle(index - 1)
            });
        }
        
        elements.AddRange(
        items
            .Skip(index * Constants.ITEMS_PER_PAGE)
            .Take(Constants.ITEMS_PER_PAGE)
            .Select(item =>
            {
                var i = item.GetItem();

                string name = Constants.UNKNOWN;
                if (i is not null)
                    name = $"{i.itemName} (${item.SCRAP_VALUE})";
                
                return new CursorElement
                {
                    Name = name,
                    Action = () =>
                    {
                        string message = Constants.TEXT_SINGLE_RETRIEVE;

                        message = string.Format(message, name);
                        Confirm(message, () =>
                        {
                            ChuteInteract.Instance?.SpawnItemServerRpc(item);
                            UnityEngine.Object.Destroy(InteractiveTerminalManager.Instance);
                        });
                    }
                };
            })
        );
        
        // Add next button
        if (items.Count() > Constants.ITEMS_PER_PAGE * (index + 1))
        {
            elements.Add(new CursorElement
            {
                Name = Constants.NEXT,
                Action = () => RetrieveSingle(index + 1)
            });
        }
        
        var options = new CursorMenu
        {
            cursorIndex = 0,
            elements = elements.ToArray()
        };
        var screen = new BoxedScreen
        {
            Title = "Inventory",
            elements = [
                TextElement.Create("Simply select the desired item to retrieve it."),
                TextElement.Create(" "),
                options
            ],
        };
        SwitchScreen(screen, options, true);
    }
    private void RetrieveType(int index)
    {
        var items = ItemManager.GetItems().GroupBy(i => i.ID);
        var elements = new List<CursorElement>();
        
        // Add previous button
        if (index > 0)
        {
            elements.Add(new CursorElement
            {
                Name = Constants.PREVIOUS,
                Action = () => RetrieveType(index - 1)
            });
        }
        
        elements.AddRange(
        items
            .Skip(index * Constants.ITEMS_PER_PAGE)
            .Take(Constants.ITEMS_PER_PAGE)
            .Select(group =>
            {
                var i = group.First().GetItem();
                var amount = group.Count();

                string name = Constants.UNKNOWN;
                if (i is not null)
                    name = $"{i.itemName} x{amount} (${group.Sum(data => data.SCRAP_VALUE)})";
                
                return new CursorElement
                {
                    Name = name,
                    Action = () =>
                    {
                        string message = Constants.TEXT_TYPE_RETRIEVE;

                        message = string.Format(message, amount, i?.itemName ?? Constants.UNKNOWN);
                        Confirm(message, () =>
                        {
                            ChuteInteract.Instance?.SpawnItemServerRpc(
                                group.First(),
                                amount
                            );
                            UnityEngine.Object.Destroy(InteractiveTerminalManager.Instance);
                        });
                    }
                };
            })
        );
        
        // Add next button
        if (items.Count() > Constants.ITEMS_PER_PAGE * (index + 1))
        {
            elements.Add(new CursorElement
            {
                Name = Constants.NEXT,
                Action = () => RetrieveType(index + 1)
            });
        }
        
        var options = new CursorMenu
        {
            cursorIndex = 0,
            elements = elements.ToArray()
        };
        var screen = new BoxedScreen {
            Title = "Inventory",
            elements = [
                TextElement.Create("Simply select the desired type to retrieve all the items of this type."),
                TextElement.Create(" "),
                options
            ],
        };
        SwitchScreen(screen, options, true);
    }
}