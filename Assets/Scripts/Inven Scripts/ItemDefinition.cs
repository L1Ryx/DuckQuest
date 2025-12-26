using UnityEngine;

public abstract class ItemDefinition : ScriptableObject
{
    [Tooltip("Stable unique ID used for save/load. Keep constant once created.")]
    public string itemId;

    [Header("Presentation")]
    public string displayName;

    [TextArea(2, 4)]
    public string description;

    public Sprite icon;
}