using UnityEngine;

public abstract class ItemDefinition : ScriptableObject
{
    [Tooltip("Stable unique ID used for save/load. Keep constant once created.")]
    public string itemId;

    public string displayName;
    public Sprite icon;
}