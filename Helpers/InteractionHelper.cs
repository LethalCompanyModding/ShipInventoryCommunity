using System;
using System.Collections.Generic;
using GameNetcodeStuff;

namespace ShipInventory.Helpers;

public static class InteractionHelper
{
    static InteractionHelper()
    {
        AddCondition(IsHoldingObject, Lang.Get("NOT_HOLDING_ITEM"));
        AddCondition(HasPermission, Lang.Get("CHUTE_PERMISSION_MISSING"));
        AddCondition(RequireInOrbit, Lang.Get("NOT_IN_ORBIT"));
        AddCondition(HasFreeSpace, Lang.Get("INVENTORY_FULL"));
        AddCondition(IsAllowed, Lang.Get("ITEM_BLACKLISTED"));
        AddCondition(IsValid, Lang.Get("ITEM_NOT_ALLOWED"));
    }
    
    public static void UpdateChuteTrigger(this InteractTrigger trigger, PlayerControllerB local)
    {
        foreach (var (condition, error) in triggerConditions)
        {
            if (condition.Invoke(local))
                continue;

            trigger.interactable = false;
            trigger.disabledHoverTip = error;
            return;
        }

        trigger.interactable = true;
        trigger.disabledHoverTip = "";
    }

    #region Conditions

    private static readonly List<(Func<PlayerControllerB, bool>, string)> triggerConditions = [];

    /// <summary>
    /// Adds a condition that defines if the chute is opened or not
    /// </summary>
    /// <param name="condition">Returns true if the condition allows the item</param>
    /// <param name="error">Error message to display</param>
    /// <param name="appendToStart"></param>
    public static void AddCondition(Func<PlayerControllerB, bool> condition, string error, bool appendToStart = false)
    {
        int index = appendToStart ? 0 : triggerConditions.Count;
        
        triggerConditions.Insert(index, (condition, error));
    }

    private static bool IsHoldingObject(PlayerControllerB p) => p.isHoldingObject && p.currentlyHeldObjectServer != null;

    private static bool HasPermission(PlayerControllerB p)
    {
        var permission = ShipInventory.Config.ChutePermission.Value;

        switch (permission)
        {
            case Config.PermissionLevel.NO_ONE:
            case Config.PermissionLevel.HOST_ONLY when !p.IsHost:
            case Config.PermissionLevel.CLIENTS_ONLY when p.IsHost:
                return false;
            case Config.PermissionLevel.EVERYONE:
            default:
                return true;
        }
    }

    private static bool RequireInOrbit(PlayerControllerB p) => !ShipInventory.Config.RequireInOrbit.Value || StartOfRound.Instance.inShipPhase;

    private static bool HasFreeSpace(PlayerControllerB p) => ItemManager.GetCount() < ShipInventory.Config.MaxItemCount.Value;

    private static bool IsAllowed(PlayerControllerB p) => !ItemManager.IsBlacklisted(p.currentlyHeldObjectServer.itemProperties);

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
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (properties.spawnPrefab.TryGetComponent(out RagdollGrabbableObject _))
            return false;
        return true;
    }

    #endregion
}