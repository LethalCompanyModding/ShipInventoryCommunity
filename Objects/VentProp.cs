namespace ShipInventory.Objects;

public class VentProp : GrabbableObject
{
    public override void Start()
    {
        base.Start();
        
        if (radarIcon != null)
            Destroy(radarIcon.gameObject);
    }
    
    public override void Update()
    {
    }
}