using System.Linq;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using ShipInventory.Helpers;

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
            InfoCursorElement()
        };

        var optionMenu = new CursorMenu {
            cursorIndex = ItemManager.GetItems().Any() ? 0 : (elements.Length - 1),
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
}