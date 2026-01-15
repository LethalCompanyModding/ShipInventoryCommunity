using HarmonyLib;
using ShipInventoryUpdated.Scripts;

// ReSharper disable InconsistentNaming

namespace ShipInventoryUpdated.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal class RoundManager_Patches
{
	[HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
	[HarmonyPostfix]
	private static void DespawnPropsAtEndOfRound_Postfix(RoundManager __instance)
	{
		if (!__instance.IsServer)
			return;

		var itemsToUpdate = Inventory.Items.Where(item => !item.PERSISTED_THROUGH_ROUNDS).ToArray();

		Inventory.MarkPersisted(itemsToUpdate);
	}
}