using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Hardworm Pack Definition")]
public class HardwormPackDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Stable unique ID used for save/load. Keep constant once created.")]
    public string itemId;

    [Header("Gameplay")]
    [Range(1, 5)]
    public int packSize = 1;

    [Header("Presentation")]
    public string displayName;
    public Sprite icon;
}