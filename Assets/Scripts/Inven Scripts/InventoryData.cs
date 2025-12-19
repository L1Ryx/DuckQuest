using System;
using System.Collections.Generic;

[Serializable]
public struct InventoryEntry
{
    public string itemId;
    public int count; 
}

[Serializable]
public class InventoryData
{
    public List<InventoryEntry> entries = new();
}