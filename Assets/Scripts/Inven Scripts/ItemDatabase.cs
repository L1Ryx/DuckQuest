using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDefinition> items = new();
    public IReadOnlyList<ItemDefinition> Items => items;

    private Dictionary<string, ItemDefinition> byId;

    public void Init()
    {
        byId = new Dictionary<string, ItemDefinition>(items.Count);

        foreach (var def in items)
        {
            if (def == null) continue;
            if (string.IsNullOrWhiteSpace(def.itemId)) continue;

            byId[def.itemId] = def;
        }
    }

    public T Get<T>(string itemId) where T : ItemDefinition
    {
        if (byId == null) Init();
        return byId.TryGetValue(itemId, out var def) ? def as T : null;
    }

    public ItemDefinition Get(string itemId)
    {
        if (byId == null) Init();
        return byId.TryGetValue(itemId, out var def) ? def : null;
    }

    public IReadOnlyList<ItemDefinition> AllItems => items;
}