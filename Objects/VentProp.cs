using System.Linq;

namespace ShipInventory.Objects;

public class VentProp : GrabbableObject
{
    public override void Start()
    {
        base.Start();
        
        if (radarIcon != null)
            Destroy(radarIcon.gameObject);
    }
    
    public override void Update() { /* Cancel the update for grabbable */ }
    
    public static void RemoveChute(ref GrabbableObject[] list)
    {
        list = list.Where(o => o is not VentProp).ToArray();
    }
}