using System;
using System.Collections.Generic;

public class InventoryModel
{
    public InventoryData Data { get; }
    public event Action OnChanged;

    // internal fast lookup: itemId -> index in entries list
    private readonly Dictionary<string, int> indexById = new();

    public InventoryModel(InventoryData data)
    {
        Data = data;
        RebuildIndex();
    }

    public int GetCount(string itemId)
    {
        return indexById.TryGetValue(itemId, out var idx) ? Data.entries[idx].count : 0;
    }

    public void Add(string itemId, int packsToAdd = 1)
    {
        if (packsToAdd <= 0) return;

        if (indexById.TryGetValue(itemId, out var idx))
        {
            var e = Data.entries[idx];
            e.count += packsToAdd;
            Data.entries[idx] = e;
        }
        else
        {
            Data.entries.Add(new InventoryEntry { itemId = itemId, count = packsToAdd });
            indexById[itemId] = Data.entries.Count - 1;
        }

        OnChanged?.Invoke();
    }

    public bool TryRemove(string itemId, int packsToRemove = 1)
    {
        if (packsToRemove <= 0) return true;

        if (!indexById.TryGetValue(itemId, out var idx))
            return false;

        var e = Data.entries[idx];
        if (e.count < packsToRemove)
            return false;

        e.count -= packsToRemove;

        if (e.count == 0)
        {
            // Remove entry, keep list compact
            Data.entries.RemoveAt(idx);
            RebuildIndex();
        }
        else
        {
            Data.entries[idx] = e;
        }

        OnChanged?.Invoke();
        return true;
    }

    private void RebuildIndex()
    {
        indexById.Clear();
        for (int i = 0; i < Data.entries.Count; i++)
        {
            indexById[Data.entries[i].itemId] = i;
        }
    }
}