using System.Runtime.CompilerServices;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using ShipInventoryUpdated.Configurations;
using ShipInventoryUpdated.Helpers;
using UnityEngine;
using Logger = ShipInventoryUpdated.Helpers.Logger;

namespace ShipInventoryUpdated.Dependencies.LethalConfig;

internal static class Dependency
{
    public static bool Enabled => Helpers.Dependencies.IsEnabled(PluginInfo.Guid);

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ApplyConfiguration(Configuration? configuration)
    {
        ApplyInformation();

        if (configuration != null)
        {
            LethalConfigManager.SkipAutoGen();
            ApplyTerminalConfiguration(configuration.Terminal);
        }
        else
            Logger.Info($"Tried to load the configurations into '{nameof(LethalConfig)}', but none were given.");
    }

    private static void ApplyInformation()
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

    private static void ApplyTerminalConfiguration(TerminalConfig config)
    {
        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(
            config.InventoryCommand,
            new TextInputFieldOptions
            {
                Name = Localization.Get("configuration.terminal.command.name"),
                NumberOfLines = 1,
                CharacterLimit = 64,
                TrimText = true,
                RequiresRestart = false
            }
        ));
    }
}