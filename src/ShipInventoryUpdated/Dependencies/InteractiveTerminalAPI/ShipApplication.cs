using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Page;
using InteractiveTerminalAPI.UI.Screen;
using ShipInventoryUpdated.Configurations;
using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Objects;
using ShipInventoryUpdated.Patches;
using ShipInventoryUpdated.Scripts;
using UnityEngine.InputSystem;

namespace ShipInventoryUpdated.Dependencies.InteractiveTerminalAPI;

public class ShipApplication : PageApplication<CursorElement>
{
	#region Application

	/// <inheritdoc/>
	public override void Initialization()
	{
		if (!Terminal_Patches.IsChuteUnlocked())
		{
			LockedScreen();
			return;
		}

		MainScreen();
	}

	/// <inheritdoc/>
	protected override int GetEntriesPerPage<T>(T[] entries) => 10;

	#endregion

	#region Exit Action

	private Action<InputAction.CallbackContext>? _lastExitPerformedAction;

	private void RegisterExitAction(Action<InputAction.CallbackContext> action)
	{
		UnregisterExitAction();
		_lastExitPerformedAction = action;
		global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed -= OnScreenExit;
		global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed += action;
	}

	private void UnregisterExitAction()
	{
		if (_lastExitPerformedAction != null)
		{
			global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed -= _lastExitPerformedAction;
			_lastExitPerformedAction = null;
		}
		// If OnScreenExit is not already registered, this is a no-op
		// Ensures OnScreenExit is never double-registered
		global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed -= OnScreenExit;
		global::InteractiveTerminalAPI.Compat.InputUtils_Compat.CursorExitKey.performed += OnScreenExit;
	}

	#endregion

	#region Utils

	private static ItemData[] GetItems(bool applySortOrder)
	{
		var items = Inventory.Items;

		if (!applySortOrder)
			return items;

		var order = Configuration.Instance?.Terminal.InventorySortOrder.Value ?? TerminalConfig.SortOrder.NONE;
		var defaultName = Localization.Get("application.answers.unknown");

		IEnumerable<ItemData> orderedItems = order switch
		{
			TerminalConfig.SortOrder.NAME_ASC   => items.OrderBy(i => i.GetItem()?.itemName ?? defaultName),
			TerminalConfig.SortOrder.NAME_DESC  => items.OrderByDescending(i => i.GetItem()?.itemName ?? defaultName),
			TerminalConfig.SortOrder.VALUE_ASC  => items.OrderBy(i => i.SCRAP_VALUE),
			TerminalConfig.SortOrder.VALUE_DESC => items.OrderByDescending(i => i.SCRAP_VALUE),
			_                                   => items
		};

		return orderedItems.ToArray();
	}

	private void CreateItemPages<T>(
		T[]                                   items,
		int                                   selectedIndex,
		string                                text,
		Func<T, (string, string, ItemData[])> render,
		Action<int>                           action
	)
	{
		var entriesPerPage = GetEntriesPerPage<int>([]);

		var cursorCount = items.Length;
		(T[][] pageGroups, BaseCursorMenu<CursorElement>[] cursorMenus, IScreen[] screens) = GetPageEntries(items);

		for (var i = 0; i < pageGroups.Length; i++)
		{
			var elements = new CursorElement[pageGroups[i].Length];

			for (var j = 0; j < elements.Length; j++)
			{
				var data = pageGroups[i][j];

				// It's normal to have default entries (the array is page-sized, but there may be fewer entries)
				if (data == null || data.Equals(default(T)))
					continue;

				var result = render.Invoke(data);
				var itemIndex = i * entriesPerPage + j;

				elements[j] = new CursorElement
				{
					Name = result.Item1,
					Action = () =>
					{
						ConfirmElement(result.Item2,
							() =>
							{
								Inventory.Remove(result.Item3);

								if (cursorCount == 1)
									MainScreen();
								else
									action.Invoke(itemIndex);
							},
							() => action.Invoke(itemIndex));
					}
				};
			}

			cursorMenus[i] = new CursorMenu<CursorElement>
			{
				cursorIndex = 0,
				elements = elements
			};

			screens[i] = CreateScreen(Localization.Get("application.titles.selection"),
				[
					TextElement.Create(text),
					TextElement.Create(" "),
					cursorMenus[i]
				]
			);
		}

		selectedIndex = Math.Clamp(selectedIndex, 0, cursorCount - 1);

		var currentPageIndex = selectedIndex / entriesPerPage;
		var currentCursorIndex = selectedIndex % entriesPerPage;

		// Set the current page's cursor
		cursorMenus[currentPageIndex].cursorIndex = currentCursorIndex;

		currentPage = PageCursorElement<CursorElement>.Create(currentPageIndex, screens, cursorMenus);
		currentCursorMenu = currentPage.GetCurrentCursorMenu();
		currentScreen = currentPage.GetCurrentScreen();

		RegisterExitAction(_ => MainScreen());
	}

	#endregion

	#region Locked Screen

	private void LockedScreen()
	{
		var optionMenu = new CursorMenu<CursorElement>
		{
			cursorIndex = 0,
			elements =
			[
				new CursorElement
				{
					Name = Localization.Get("application.answers.blocked"),
					Action = () => OnScreenExit(new InputAction.CallbackContext())
				}
			]
		};

		var screen = CreateScreen(Localization.Get("application.titles.locked"),
			[
				TextElement.Create(Localization.Get("application.screens.locked.message")),
				TextElement.Create(" "),
				TextElement.Create(Localization.Get("application.screens.locked.tip",
					new Dictionary<string, string>
					{
						["command"] = Configuration.Instance?.Unlock.UnlockName.Value ?? ""
					})),
				TextElement.Create(" "),
				TextElement.Create(" "),
				TextElement.Create(Localization.Get("application.screens.locked.offer")),
				TextElement.Create(" "),
				optionMenu
			]
		);
		currentPage = PageCursorElement<CursorElement>.Create(0, [screen], [optionMenu]);
		SwitchScreen(screen, optionMenu, true);
	}

	#endregion

	#region Main Screen

	private void MainScreen()
	{
		var player = StartOfRound.Instance.localPlayerController;

		var elements = new[]
		{
			RetrieveSingleElement(),
			RetrieveTypeElement(),
			RetrieveRandomElement(),
			RetrieveAllElement(),
			InfoCursorElement()
		};

		var optionMenu = new CursorMenu<CursorElement>
		{
			cursorIndex = Inventory.Count > 0 ? 0 : elements.Length - 1,
			elements = elements
		};

		var screen = CreateScreen(Localization.Get("application.titles.main"),
			[
				TextElement.Create(Localization.Get("application.screens.main.welcome",
					new Dictionary<string, string>
					{
						["level"] = HUDManager.Instance.playerLevels[player.playerLevelNumber].levelName,
						["username"] = player.playerUsername
					})),
				TextElement.Create(" "),
				TextElement.Create(Localization.Get("application.screens.main.home")),
				TextElement.Create(" "),
				optionMenu
			]
		);
		currentPage = PageCursorElement<CursorElement>.Create(0, [screen], [optionMenu]);
		SwitchScreen(screen, optionMenu, true);
	}

	#endregion

	#region Confirm Screen

	private Action? _confirmExitCallback;

	private void ConfirmElement(string message, Action? confirmCallback, Action? declineCallback = null)
	{
		var showConfirmation = Configuration.Instance?.Terminal.ShowConfirmation.Value ?? true;

		if (!showConfirmation)
		{
			confirmCallback?.Invoke();
			return;
		}

		_confirmExitCallback = declineCallback;

		// Elements
		var automaticPositive = Configuration.Instance?.Terminal.AutomaticPositiveAnswer.Value ?? false;

		var optionMenu = new CursorMenu<CursorElement>
		{
			cursorIndex = automaticPositive ? 1 : 0,
			elements =
			[
				new CursorElement
				{
					Name = Localization.Get("application.answers.negative"),
					Action = () =>
					{
						if (declineCallback != null)
							declineCallback.Invoke();
						else
							MainScreen();
					}
				},
				new CursorElement
				{
					Name = Localization.Get("application.answers.positive"),
					Action = () => confirmCallback?.Invoke()
				}
			]
		};

		// Screen
		var screen = CreateScreen(Localization.Get("application.titles.confirm"),
			[
				TextElement.Create(message),
				TextElement.Create(" "),
				TextElement.Create(Localization.Get("application.screens.confirm.message")),
				TextElement.Create(" "),
				TextElement.Create(Localization.Get("application.screens.confirm.warning")),
				TextElement.Create(" "),
				optionMenu
			]
		);

		currentPage = PageCursorElement<CursorElement>.Create(0, [screen], [optionMenu]);
		SwitchScreen(screen, optionMenu, true);

		RegisterExitAction(_ => _confirmExitCallback?.Invoke());
	}

	private BoxedScreen CreateScreen(string title, ITextElement[] elements)
	{
		UnregisterExitAction();

		var showTrademark = Configuration.Instance?.Terminal.ShowTrademark.Value ?? true;

		if (showTrademark)
		{
			var copyElements = new List<ITextElement>(elements)
			{
				TextElement.Create(" "),
				TextElement.Create(Localization.Get("application.footers.trademark"))
			};

			elements = copyElements.ToArray();
		}

		return new BoxedScreen
		{
			Title = title,
			elements = elements
		};
	}

	#endregion

	#region Retrieve Single Screen

	private CursorElement RetrieveSingleElement() => new()
	{
		Name = Localization.Get("application.titles.single"),
		Active = _ => Inventory.Count > 0,
		SelectInactive = false,
		Action = () => RetrieveSingle()
	};

	private static (string, string, ItemData[]) RenderSingle(ItemData itemData)
	{
		var name = itemData.GetItem()?.itemName ?? Localization.Get("application.answers.unknown");

		var itemFormat = Localization.Get("application.screens.single.item",
			new Dictionary<string, string>
			{
				["name"] = name,
				["value"] = itemData.SCRAP_VALUE.ToString()
			});

		var message = Localization.Get("application.screens.single.confirm",
			new Dictionary<string, string>
			{
				["name"] = name
			});

		return (itemFormat, message, [itemData]);
	}

	private void RetrieveSingle(int selectedIndex = 0) => CreateItemPages(
		GetItems(true),
		selectedIndex,
		Localization.Get("application.screens.single.message"),
		RenderSingle,
		RetrieveSingle
	);

	#endregion

	#region Retrieve Type Screen

	private CursorElement RetrieveTypeElement() => new()
	{
		Name = Localization.Get("application.titles.type"),
		Active = _ => Inventory.Count > 0,
		SelectInactive = false,
		Action = () => RetrieveType()
	};

	private static (string, string, ItemData[]) RenderType(IGrouping<string, ItemData> group)
	{
		var amount = group.Count().ToString();

		var name = group.First().GetItem()?.itemName ?? Localization.Get("application.answers.unknown");

		var itemFormat = Localization.Get("application.screens.type.item",
			new Dictionary<string, string>
			{
				["name"] = name,
				["amount"] = amount,
				["total"] = group.Sum(d => d.SCRAP_VALUE).ToString()
			});

		var message = Localization.Get("application.screens.type.confirm",
			new Dictionary<string, string>
			{
				["name"] = name,
				["amount"] = amount
			});

		return (itemFormat, message, group.ToArray());
	}

	private void RetrieveType(int selectedIndex = 0) => CreateItemPages(
		GetItems(true).GroupBy(i => i.ID.ToString()).ToArray(),
		selectedIndex,
		Localization.Get("application.screens.single.message"),
		RenderType,
		RetrieveType
	);

	#endregion

	#region Retrieve Random Screen

	private CursorElement RetrieveRandomElement() => new()
	{
		Name = Localization.Get("application.titles.random"),
		Active = _ => Inventory.Count > 0,
		SelectInactive = false,
		Action = RetrieveRandom
	};

	private void RetrieveRandom()
	{
		// Random object
		var data = Inventory.Items[UnityEngine.Random.Range(0, Inventory.Count)];

		// Generate message
		var message = Localization.Get("application.screens.random.message",
			new Dictionary<string, string>
			{
				["name"] = data.GetItem()?.itemName ?? Localization.Get("application.answers.unknown")
			});

		ConfirmElement(message,
			() =>
			{
				// Spawn random
				Inventory.Remove([data]);

				MainScreen();
			});

		RegisterExitAction(_ => MainScreen());
	}

	#endregion

	#region Retrieve All Screen

	private CursorElement RetrieveAllElement() => new()
	{
		Name = Localization.Get("application.titles.all"),
		Active = _ => Inventory.Count > 0,
		SelectInactive = false,
		Action = RetrieveAll
	};

	private void RetrieveAll()
	{
		var items = GetItems(false);

		var message = Localization.Get("application.screens.all.message",
			new Dictionary<string, string>
			{
				["amount"] = items.Length.ToString(),
				["total"] = items.Sum(i => i.SCRAP_VALUE).ToString()
			});

		ConfirmElement(message,
			() =>
			{
				Inventory.Remove(items);
				MainScreen();
			});

		RegisterExitAction(_ => MainScreen());
	}

	#endregion

	#region Info Screen

	private CursorElement InfoCursorElement() => new()
	{
		Name = Localization.Get("application.titles.information"),
		Action = GetInfo
	};

	private void GetInfo()
	{
		var maxCount = Configuration.Instance?.Inventory.MaxItemCount.Value ?? 1;

		var items = Inventory.Items;

		var options = new CursorMenu<CursorElement>
		{
			cursorIndex = 0,
			elements = []
		};

		var screen = CreateScreen(Localization.Get("application.titles.status"),
			[
				TextElement.Create(Localization.Get("application.screens.information.total",
					new Dictionary<string, string>
					{
						["total"] = items.Sum(i => i.SCRAP_VALUE).ToString()
					})),
				TextElement.Create(Localization.Get("application.screens.information.count",
					new Dictionary<string, string>
					{
						["count"] = items.Length.ToString(),
						["maxCount"] = maxCount.ToString()
					}))
			]
		);

		currentPage = PageCursorElement<CursorElement>.Create(0, [screen], [options]);
		SwitchScreen(screen, options, true);

		RegisterExitAction(_ => MainScreen());
	}

	#endregion
}