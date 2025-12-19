using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<HardwormPackDefinition> hardwormPacks = new();

    private Dictionary<string, HardwormPackDefinition> byId;

    public void Init()
    {
        byId = new Dictionary<string, HardwormPackDefinition>(hardwormPacks.Count);
        foreach (var def in hardwormPacks)
        {
            if (def == null || string.IsNullOrWhiteSpace(def.itemId)) continue;
            byId[def.itemId] = def;
        }
    }

    public HardwormPackDefinition Get(string itemId)
    {
        if (byId == null) Init();
        return byId.TryGetValue(itemId, out var def) ? def : null;
    }

    public IReadOnlyList<HardwormPackDefinition> AllHardwormPacks => hardwormPacks;
}