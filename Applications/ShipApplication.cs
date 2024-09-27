using System.Collections.Generic;
using System.Linq;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Applications;

public class ShipApplication : InteractiveTerminalApplication
{
    #region General Texts

    private readonly string INVENTORY_TITLE = Lang.Get("INVENTORY_TITLE");
    private readonly string TERMINAL_TITLE = Lang.Get("TERMINAL_TITLE");

    private readonly string NEXT = Lang.Get("NEXT");
    private readonly string PREVIOUS = Lang.Get("PREVIOUS");
    private readonly string UNKNOWN = Lang.Get("UNKNOWN");
    
    #endregion
    
    /// <inheritdoc/>
    public override void Initialization() => MainScreen();

    #region Main Screen
    
    private readonly string HOME_MESSAGE = Lang.Get("HOME_MESSAGE");
    private readonly string WELCOME_MESSAGE = Lang.Get("WELCOME_MESSAGE");

    private readonly string TRADEMARK = Lang.Get("TRADEMARK");

    private void MainScreen()
    {
        var player = StartOfRound.Instance.localPlayerController;

        var elements = new[]
        {
            RetrieveSingleElement(),
            RetrieveTypeElement(),
            RetrieveRandomElement(),
            RetrieveAllElement(),
            InfoCursorElement(),
        };

        var optionMenu = new CursorMenu {
            cursorIndex = ItemManager.GetItems().Any() ? 0 : elements.Length - 1,
            elements = elements
        };

        var screen = new BoxedScreen {
            Title = TERMINAL_TITLE,
            elements =
            [
                TextElement.Create(string.Format(
                    WELCOME_MESSAGE,
                    HUDManager.Instance.playerLevels[player.playerLevelNumber].levelName,
                    player.playerUsername
                )),
                TextElement.Create(" "),
                TextElement.Create(HOME_MESSAGE),
                TextElement.Create(" "),
                optionMenu,
                TextElement.Create(" "),
                TextElement.Create(TRADEMARK)
            ]
        };
        SwitchScreen(screen, optionMenu, true);
    }

    #endregion

    #region Utils

    private readonly string POSITIVE_ANSWER = Lang.Get("POSITIVE_ANSWER");
    private readonly string NEGATIVE_ANSWER = Lang.Get("NEGATIVE_ANSWER");
    
    private string BoolToString(bool value) => value
        ? $"<color=green>{POSITIVE_ANSWER}</color>"
        : $"<color=red>{NEGATIVE_ANSWER}</color>";
    
    private readonly string EXIT = Lang.Get("EXIT");

    private CursorElement ExitElement() => new()
    {
        Name = EXIT,
        Action = MainScreen
    };
    
    private readonly string CONFIRMATION_TITLE = Lang.Get("CONFIRMATION_TITLE");
    private readonly string CONFIRMATION_MESSAGE = Lang.Get("CONFIRMATION_MESSAGE");
    
    private void ConfirmElement(string message, System.Action? callback)
    {
        // If skip confirmation, skip
        if (!ShipInventory.Config.ShowConfirmation.Value)
        {
            callback?.Invoke();
            return;
        }
            
        var optionMenu = new CursorMenu
        {
            cursorIndex = ShipInventory.Config.YesPlease.Value ? 1 : 0,
            elements =
            [
                new CursorElement
                {
                    Name = NEGATIVE_ANSWER,
                    Action = MainScreen
                },
                new CursorElement
                {
                    Name = POSITIVE_ANSWER,
                    Action = () => callback?.Invoke()
                }
            ]
        };
    
        var screen = new BoxedScreen {
            Title = CONFIRMATION_TITLE,
            elements =
            [
                TextElement.Create(message),
                TextElement.Create(" "),
                TextElement.Create(CONFIRMATION_MESSAGE),
                TextElement.Create(" "),
                optionMenu
            ]
        };
        SwitchScreen(screen, optionMenu, true);
    }

    #endregion
    
    #region Info Screen

    private readonly string SHIP_INFO = Lang.Get("SHIP_INFO");

    private readonly string SHIP_INFO_HEADER = Lang.Get("SHIP_INFO_HEADER");
    private readonly string SHIP_INFO_TOTAL = Lang.Get("SHIP_INFO_TOTAL");
    private readonly string SHIP_INFO_COUNT = Lang.Get("SHIP_INFO_COUNT");
    private readonly string SHIP_INFO_IS_SAFE = Lang.Get("SHIP_INFO_IS_SAFE");
    private readonly string SHIP_INFO_IN_ORBIT = Lang.Get("SHIP_INFO_IN_ORBIT");
    
    private CursorElement InfoCursorElement() => new()
    {
        Name = SHIP_INFO,
        Action = GetInfo
    };
    
    private void GetInfo()
    {
        var options = new CursorMenu
        {
            cursorIndex = 0,
            elements = [ ExitElement() ]
        };
        
        var screen = new BoxedScreen
        {
            Title = INVENTORY_TITLE,
            elements = [
                TextElement.Create(SHIP_INFO_HEADER),
                TextElement.Create(" "),
                TextElement.Create(string.Format(SHIP_INFO_TOTAL, ItemManager.GetTotalValue())),
                TextElement.Create(string.Format(
                    SHIP_INFO_COUNT, 
                    ItemManager.GetItems().Count(),
                    ShipInventory.Config.MaxItemCount.Value
                )), 
                TextElement.Create(" "),
                TextElement.Create(string.Format(SHIP_INFO_IS_SAFE, BoolToString(ShipInventory.Config.ActAsSafe.Value))),
                TextElement.Create(string.Format(SHIP_INFO_IN_ORBIT, BoolToString(!ShipInventory.Config.RequireInOrbit.Value))),
                TextElement.Create(" "),
                options
            ]
        };
        
        SwitchScreen(screen, options, true);
    }

    #endregion
    
    #region Retrieve Single Screen

    private readonly string SINGLE_RETRIEVE = Lang.Get("SINGLE_RETRIEVE");
    private readonly string TEXT_SINGLE_RETRIEVE = Lang.Get("TEXT_SINGLE_RETRIEVE");
    private readonly string SINGLE_RETRIEVE_MESSAGE = Lang.Get("SINGLE_RETRIEVE_MESSAGE");
    private readonly string SINGLE_ITEM_FORMAT = Lang.Get("SINGLE_ITEM_FORMAT");
    
    private CursorElement RetrieveSingleElement() => new()
    {
        Name = SINGLE_RETRIEVE,
        Action = () => RetrieveSingle(0)
    };
    
    private List<CursorElement> GetSinglePage(int index)
    {
        var items = ItemManager.GetItems();
        var elements = new List<CursorElement>();
        
        // Add previous button
        if (index > 0)
        {
            elements.Add(new CursorElement
            {
                Name = PREVIOUS,
                Action = () => RetrieveSingle(index - 1)
            });
        }

        foreach (var item in items.Skip(index * Constants.ITEMS_PER_PAGE).Take(Constants.ITEMS_PER_PAGE))
        {
            var i = item.GetItem();

            string name = UNKNOWN;
            if (i is not null)
            {
                name = string.Format(
                    SINGLE_ITEM_FORMAT,
                    i.itemName,
                    item.SCRAP_VALUE
                );
            }
                
            var groupElement = new CursorElement
            {
                Name = name,
                Action = () =>
                {
                    string message = TEXT_SINGLE_RETRIEVE;

                    message = string.Format(message, name);
                    ConfirmElement(message, () =>
                    {
                        ChuteInteract.Instance?.SpawnItemServerRpc(item);
                        UnityEngine.Object.Destroy(InteractiveTerminalManager.Instance);
                    });
                }
            };
                
            elements.Add(groupElement);
        }
        
        // Add next button
        if (items.Count() > Constants.ITEMS_PER_PAGE * (index + 1))
        {
            elements.Add(new CursorElement
            {
                Name = NEXT,
                Action = () => RetrieveSingle(index + 1)
            });
        }

        return elements;
    }

    private void RetrieveSingle(int index)
    {
        var options = new CursorMenu
        {
            cursorIndex = 0,
            elements = GetSinglePage(index).ToArray()
        };
        var screen = new BoxedScreen
        {
            Title = INVENTORY_TITLE,
            elements = [
                TextElement.Create(SINGLE_RETRIEVE_MESSAGE),
                TextElement.Create(" "),
                options
            ],
        };
        SwitchScreen(screen, options, true);
    }

    #endregion

    #region Retrieve Type Screen

    private readonly string TYPE_RETRIEVE = Lang.Get("TYPE_RETRIEVE");
    private readonly string TEXT_TYPE_RETRIEVE = Lang.Get("TEXT_TYPE_RETRIEVE");
    private readonly string TYPE_RETRIEVE_MESSAGE = Lang.Get("TYPE_RETRIEVE_MESSAGE");
    private readonly string TYPE_ITEM_FORMAT = Lang.Get("TYPE_ITEM_FORMAT");
    
    private CursorElement RetrieveTypeElement() => new()
    {
        Name = TYPE_RETRIEVE,
        Action = () => RetrieveType(0)
    };

    private List<CursorElement> GetTypePage(int index)
    {
        var items = ItemManager.GetItems().GroupBy(i => i.ID);
        var elements = new List<CursorElement>();
        
        // Add previous button
        if (index > 0)
        {
            elements.Add(new CursorElement
            {
                Name = PREVIOUS,
                Action = () => RetrieveType(index - 1)
            });
        }

        foreach (var group in items.Skip(index * Constants.ITEMS_PER_PAGE).Take(Constants.ITEMS_PER_PAGE))
        {
            var i = group.First().GetItem();
            var amount = group.Count();

            string name = UNKNOWN;
            if (i is not null)
            {
                name = string.Format(
                    TYPE_ITEM_FORMAT,
                    i.itemName,
                    amount,
                    group.Sum(data => data.SCRAP_VALUE)
                );
            }
                
            var groupElement = new CursorElement
            {
                Name = name,
                Action = () =>
                {
                    string message = TEXT_TYPE_RETRIEVE;

                    message = string.Format(message, amount, i?.itemName ?? UNKNOWN);
                    ConfirmElement(message, () =>
                    {
                        ChuteInteract.Instance?.SpawnItemServerRpc(
                            group.First(),
                            amount
                        );
                        UnityEngine.Object.Destroy(InteractiveTerminalManager.Instance);
                    });
                }
            };
            elements.Add(groupElement);
        }
        
        // Add next button
        if (items.Count() > Constants.ITEMS_PER_PAGE * (index + 1))
        {
            elements.Add(new CursorElement
            {
                Name = NEXT,
                Action = () => RetrieveType(index + 1)
            });
        }

        return elements;
    }
    
    private void RetrieveType(int index)
    {
        var options = new CursorMenu
        {
            cursorIndex = 0,
            elements = GetTypePage(index).ToArray()
        };
        
        var screen = new BoxedScreen {
            Title = INVENTORY_TITLE,
            elements = [
                TextElement.Create(TYPE_RETRIEVE_MESSAGE),
                TextElement.Create(" "),
                options
            ],
        };
        
        SwitchScreen(screen, options, true);
    }

    #endregion

    #region Retrieve Random Screen

    private readonly string RANDOM_RETRIEVE = Lang.Get("RANDOM_RETRIEVE");
    private readonly string TEXT_RANDOM_RETRIEVE = Lang.Get("TEXT_RANDOM_RETRIEVE");
    
    private CursorElement RetrieveRandomElement() => new()
    {
        Name = RANDOM_RETRIEVE,
        Action = RetrieveRandom
    };
    
    private void RetrieveRandom()
    {
        // Random object
        var items = ItemManager.GetItems();

        object randomObj = items.ElementAt(UnityEngine.Random.Range(0, items.Count()));
        
        // Generate message
        string message = TEXT_RANDOM_RETRIEVE;

        string name = randomObj switch
        {
            ItemData data => data.GetItem()?.itemName,
            EnemyType enemyType => enemyType.enemyName,
            _ => null
        } ?? UNKNOWN;

        message = string.Format(message, name);
        
        ConfirmElement(message, () =>
        {
            // Spawn random
            if (randomObj is ItemData data)
                ChuteInteract.Instance?.SpawnItemServerRpc(data);

            UnityEngine.Object.Destroy(InteractiveTerminalManager.Instance);
        });
    }

    #endregion

    #region Retrieve All Screen

    private readonly string ALL_RETRIEVE = Lang.Get("ALL_RETRIEVE");
    private readonly string TEXT_ALL_RETRIEVE = Lang.Get("TEXT_ALL_RETRIEVE");
    
    private CursorElement RetrieveAllElement() => new()
    {
        Name = ALL_RETRIEVE,
        Action = RetrieveAll
    };
    
    private void RetrieveAll()
    {
        ConfirmElement(TEXT_ALL_RETRIEVE, () =>
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

    #endregion
}