public class InventorySelectionModel
{
    private readonly InventoryModel inv;
    private int selectedIndex;

    public InventorySelectionModel(InventoryModel inv)
    {
        this.inv = inv;
        selectedIndex = 0;

        // Keep selection valid when inventory changes
        inv.OnChanged += EnsureValid;
    }

    public int SelectedIndex => selectedIndex;

    public void CycleNext()
    {
        var entries = inv.Data.entries;
        if (entries == null || entries.Count == 0)
            return;

        selectedIndex = (selectedIndex + 1) % entries.Count;
    }

    public InventoryEntry GetSelectedEntry()
    {
        var entries = inv.Data.entries;
        if (entries == null || entries.Count == 0)
            return default;

        EnsureValid();
        return entries[selectedIndex];
    }

    public string GetSelectedItemId()
    {
        var e = GetSelectedEntry();
        return string.IsNullOrEmpty(e.itemId) ? null : e.itemId;
    }

    public void EnsureValid()
    {
        var entries = inv.Data.entries;
        if (entries == null || entries.Count == 0)
        {
            selectedIndex = 0;
            return;
        }

        if (selectedIndex < 0) selectedIndex = 0;
        if (selectedIndex >= entries.Count) selectedIndex = entries.Count - 1;
    }
}