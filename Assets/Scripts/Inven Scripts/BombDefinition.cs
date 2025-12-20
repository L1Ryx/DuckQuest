using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Bomb Definition")]
public class BombDefinition : ItemDefinition
{
    [Header("Gameplay")]
    public float fuseSeconds = 1.5f; 
}