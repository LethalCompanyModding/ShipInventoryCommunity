using System.Collections.Generic;
using ShipInventoryUpdated.Helpers;
using UnityEngine;

namespace ShipInventoryUpdated.Scripts;

[RequireComponent(typeof(Collider))]
public class ChuteTrigger : InteractTrigger
{
    #region Unity

    private new void Start()
    {
        base.Start();

        hoverTip = Localization.Get("tooltip.trigger.hover", new Dictionary<string, string>()
        {
            ["key"] = "LMB"
        });
    }

    private new void Update()
    {
        base.Update();
        
        var player = GameNetworkManager.Instance?.localPlayerController;
        
        if (player is null || !player.isInHangarShipRoom)
            return;
        
        InteractionHelper.SetTriggerStatus(this, player);
    }

    #endregion
}