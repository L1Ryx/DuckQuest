using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Hardworm Pack Definition")]
public class HardwormPackDefinition : ItemDefinition
{
    [Header("Gameplay")]
    [Range(1, 20)]
    public int packSize = 1;
}