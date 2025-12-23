using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Trade Definition")]
public class CraftingTradeDefinition : ScriptableObject
{
    [Header("Input (cost)")]
    public ItemDefinition inputItem;
    [Min(1)] public int inputCount = 1;

    [Header("Output (reward)")]
    public ItemDefinition outputItem;
    [Min(1)] public int outputCount = 1;

    [Header("Rules")]
    public bool requiresExact = true; 

}