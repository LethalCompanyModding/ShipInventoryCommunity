using System;
using System.Collections.Generic;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Page;
using InteractiveTerminalAPI.UI.Screen;
using ShipInventoryUpdated.Configurations;
using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Patches;
using UnityEngine.InputSystem;

namespace ShipInventoryUpdated.Dependencies.InteractiveTerminalAPI;

public class ShipApplication : PageApplication
{
    #region General Texts

    private static string INVENTORY_TITLE => Localization.Get("application.titles.inventory");
    private static string BLOCKED_ANSWER => Localization.Get("application.answers.blocked");

    #endregion

    #region Application

    /// <inheritdoc/>
    public override void Initialization()
    {
        if (!Terminal_Patches.IsChuteUnlocked())
        {
            LockedScreen();
            return;
        }
        
        MainScreen(0);
    }

    /// <inheritdoc/>
    protected override int GetEntriesPerPage<T>(T[] entries) => 10;

    #endregion
    
    #region Utils

    private Action<InputAction.CallbackContext>? LastExitPerformedAction;

    private void UnregisterExitAction()
    {
        if (LastExitPerformedAction != null)
        {
            global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed -= LastExitPerformedAction;
            LastExitPerformedAction = null;
        }
        // If OnScreenExit is not already registered, this is a no-op
        // Ensures OnScreenExit is never double-registered
        global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed -= OnScreenExit;
        global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed += OnScreenExit;
    }

    private BoxedScreen CreateScreen(string title, ITextElement[] elements)
    {
        UnregisterExitAction();

        return new BoxedScreen
        {
            Title = title,
            elements = elements
        };
    }

    #endregion

    #region Locked Screen

    private void LockedScreen()
    {
        var optionMenu = new CursorMenu {
            cursorIndex = 0,
            elements = [
                new CursorElement
                {
                    Name = BLOCKED_ANSWER,
                    Action = () => OnScreenExit(new InputAction.CallbackContext())
                }
            ]
        };
        
        var screen = CreateScreen(INVENTORY_TITLE,
            [
                TextElement.Create(Localization.Get("application.screens.locked.message")),
                TextElement.Create(" "),
                TextElement.Create(Localization.Get("application.screens.locked.tip", new Dictionary<string, string> {
                    ["command"] = Configuration.Instance?.Unlock.UnlockName.Value ?? ""
                })),
                TextElement.Create(" "),
                TextElement.Create(" "),
                TextElement.Create(Localization.Get("application.screens.locked.offer")),
                TextElement.Create(" "),
                optionMenu
            ]
        );
        currentPage = PageCursorElement.Create(0, [screen], [optionMenu]);
        SwitchScreen(screen, optionMenu, true);
    }

    #endregion
    
    #region Main Screen
    
    private void MainScreen(int selectedIndex)
    {
        var player = StartOfRound.Instance.localPlayerController;

        var elements = new CursorElement[]
        {
        };

        var optionMenu = new CursorMenu {
            //cursorIndex = ItemManager.HasItems() ? selectedIndex : elements.Length - 1,
            elements = elements
        };

        var screen = CreateScreen(INVENTORY_TITLE,
            [
                TextElement.Create(Localization.Get("application.screens.main.welcome", new Dictionary<string, string>
                {
                    ["level"] = HUDManager.Instance.playerLevels[player.playerLevelNumber].levelName,
                    ["username"] = player.playerUsername
                })),
                TextElement.Create(" "),
                TextElement.Create(Localization.Get("application.screens.main.home")),
                TextElement.Create(" "),
                optionMenu,
            ]
        );
        currentPage = PageCursorElement.Create(0, [screen], [optionMenu]);
        SwitchScreen(screen, optionMenu, true);
    }

    #endregion
}