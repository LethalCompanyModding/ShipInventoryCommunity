using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CustomItemBehaviourLibrary.AbstractItems;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Compatibility;

internal static class CustomItemBehaviourLibrary
{
    public const string GUID = "com.github.WhiteSpike.CustomItemBehaviourLibrary";
    private static bool? _enabled;

    public static bool Enabled
    {
        get
        {
            _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);
            return _enabled.Value;
        }
    }

    public static ItemData[]? ConvertContainer(GrabbableObject item) => Enabled ? HandleContainer(item) : null;

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static ItemData[]? HandleContainer(GrabbableObject item)
    {
        if (item is not ContainerBehaviour container)
            return null;

        container.enabled = false;

        var items = new List<ItemData>();
        var subItems = container.GetComponentsInChildren<GrabbableObject>();

        for (int i = subItems.Length - 1; i >= 0; i--)
        {
            var subItem = subItems[i];
            
            if (subItem == null)
                continue;
            
            if (subItem == container)
                continue;
            
            items.AddRange(ConvertItemHelper.ConvertItem(subItem));
        }
        
        items.Add(new ItemData(container));
        
        return items.ToArray();
    }
}