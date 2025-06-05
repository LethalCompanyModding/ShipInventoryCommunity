using System.Runtime.CompilerServices;
using LethalConfig;
using ShipInventoryUpdated.Helpers;
using UnityEngine;

namespace ShipInventoryUpdated.Dependencies.LethalConfig;

internal static class Dependency
{
    public static bool Enabled => Dependencies.IsEnabled(global::LethalConfig.PluginInfo.Guid);

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ApplyConfiguration()
    {
        var modIcon = Bundle.LoadAsset<Texture2D>("mod-icon");

        if (modIcon != null)
        {
            var sprite = Sprite.Create(
                modIcon,
                new Rect(0f, 0f, modIcon.width, modIcon.height),
                new Vector2(0.5f, 0.5f)
            );
            LethalConfigManager.SetModIcon(sprite);
        }
        
        LethalConfigManager.SetModDescription("Adds an inventory to the ship, allowing it to store items and retrieve them.");
    }
}