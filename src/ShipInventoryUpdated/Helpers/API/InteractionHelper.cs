using System.Text.RegularExpressions;
using GameNetcodeStuff;

namespace ShipInventoryUpdated.Helpers.API;

/// <summary>
/// Helper to handle the interaction condition of items
/// </summary>
public static class InteractionHelper
{
	#region API

	private static readonly List<(Func<PlayerControllerB, bool>, string)> TriggerConditions = [];

	/// <summary>
	/// Adds a condition that defines if the chute is opened or not
	/// </summary>
	/// <param name="condition">Returns true if the condition allows the item</param>
	/// <param name="error">Error message to display</param>
	public static void AddCondition(Func<PlayerControllerB, bool> condition, string error) => TriggerConditions.Add((condition, error));

	/// <summary>
	/// Sets the status of the given trigger depending on if it meets certain criteria or not
	/// </summary>
	internal static void SetTriggerStatus(InteractTrigger trigger, PlayerControllerB player)
	{
		foreach ((var condition, var error) in TriggerConditions)
		{
			if (condition.Invoke(player))
				continue;

			trigger.interactable = false;
			trigger.disabledHoverTip = error;
			return;
		}

		trigger.interactable = true;
		trigger.disabledHoverTip = "";
	}

	#endregion

	#region Conditions

	internal static void LoadConditions()
	{
		AddCondition(IsHoldingObject, Localization.Get("tooltip.trigger.emptyHand"));
		AddCondition(IsAllowed, Localization.Get("tooltip.trigger.itemBlacklisted"));
		AddCondition(IsValid, Localization.Get("tooltip.trigger.invalidItem"));
	}

	private static bool IsHoldingObject(PlayerControllerB p) => p.isHoldingObject && p.currentlyHeldObjectServer != null;

	private static bool IsAllowed(PlayerControllerB p)
	{
		var config = Configurations.Configuration.Instance;

		if (config == null)
			return true;

		var item = p.currentlyHeldObjectServer;

		if (item == null)
			return true;

		var name = item.itemProperties.itemName.ToLower();

		foreach (var s in config.Chute.Blacklist.Value.Split(',', StringSplitOptions.RemoveEmptyEntries))
		{
			if (s == null)
				continue;

			var regex = "^" + s.Trim().ToLower() + "$";

			if (Regex.IsMatch(name, regex))
				return false;
		}

		return true;
	}
	
	private static bool IsValid(PlayerControllerB p)
	{
		var item = p.currentlyHeldObjectServer;

		// Prevent used items
		if (item.itemUsedUp)
			return false;

		var properties = item.itemProperties;

		// Prevent items with no prefab
		if (properties.spawnPrefab == null)
			return false;

		// Prevent ragdoll items
		if (properties.spawnPrefab.TryGetComponent(out RagdollGrabbableObject _))
			return false;

		return true;
	}

	#endregion
}