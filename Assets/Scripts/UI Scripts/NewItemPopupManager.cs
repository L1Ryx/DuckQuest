using System.Collections.Generic;
using UnityEngine;

public class NewItemPopupManager : MonoBehaviour
{
    [Header("Prefab & Parent")]
    [SerializeField] private NewItemPopupUI popupPrefab;
    [SerializeField] private RectTransform popupParent;

    [Header("Layout")]
    [SerializeField] private RectTransform anchor;
    [SerializeField] private float verticalOffset = 60f;

    [Header("Timing")]
    [SerializeField] private float fadeInSeconds = 0.12f;
    [SerializeField] private float holdSeconds = 1.25f;
    [SerializeField] private float fadeOutSeconds = 0.18f;

    [Header("Optional")]
    [SerializeField] private bool showQuantitySuffixIfMultiple = false;

    // Active popup instances (oldest at index 0)
    private readonly List<NewItemPopupUI> activePopups = new();

    // Snapshot of last-known counts
    private readonly Dictionary<string, int> prevCounts = new();

    private void Awake()
    {
        if (popupParent == null)
            popupParent = transform as RectTransform;
        
        if (anchor == null)
            Debug.LogWarning("NewItemPopupManager: Anchor is not assigned.");
    }

    private void OnEnable()
    {
        // Prime snapshot if game is ready; otherwise the first event will populate.
        TryRefreshSnapshotFromGame();
    }

    /// <summary>
    /// Hook this to your GameEventListener UnityEvent Response
    /// for the InventoryChangedEventBridge's onInventoryChanged GameEvent.
    /// </summary>
    public void HandleInventoryChangedEvent()
    {
        if (!Game.IsReady || Game.Ctx?.Inventory == null || Game.Ctx.ItemDb == null)
            return;

        var currentCounts = BuildCounts(Game.Ctx.Inventory);

        foreach (var kvp in currentCounts)
        {
            prevCounts.TryGetValue(kvp.Key, out var previous);
            int delta = kvp.Value - previous;

            if (delta > 0)
                SpawnPopup(kvp.Key, delta);
        }

        prevCounts.Clear();
        foreach (var kvp in currentCounts)
            prevCounts[kvp.Key] = kvp.Value;
    }

    private void SpawnPopup(string itemId, int deltaAdded)
    {
        if (popupPrefab == null)
        {
            Debug.LogWarning("NewItemPopupManager: popupPrefab is not assigned.");
            return;
        }

        var def = Game.Ctx.ItemDb.Get(itemId);
        if (def == null)
        {
            Debug.LogWarning($"NewItemPopupManager: ItemDatabase has no definition for itemId '{itemId}'.");
            return;
        }

        var popup = Instantiate(popupPrefab, popupParent, false);
        popup.Bind(def);
        
        popup.RebaseFloatyIfPresent();

        activePopups.Add(popup);
        RepositionActivePopups();

        popup.Play(fadeInSeconds, holdSeconds, fadeOutSeconds, () =>
        {
            activePopups.Remove(popup);
            Destroy(popup.gameObject);
            RepositionActivePopups();
        });
    }

    private void RepositionActivePopups()
    {
        if (anchor == null)
            return;

        Vector2 basePos = anchor.anchoredPosition;

        for (int i = 0; i < activePopups.Count; i++)
        {
            var rt = activePopups[i].RectTransform;
            if (rt == null) continue;

            rt.anchoredPosition = basePos + Vector2.up * (verticalOffset * i);
        }
    }

    private Dictionary<string, int> BuildCounts(InventoryModel inventory)
    {
        var dict = new Dictionary<string, int>();
        var entries = inventory.Data.entries;

        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (!string.IsNullOrEmpty(e.itemId))
                dict[e.itemId] = e.count;
        }

        return dict;
    }

    private void RemoveMissingKeysFromPrev(Dictionary<string, int> currentCounts)
    {
        // Create list to avoid modifying while iterating
        var toRemove = ListPool<string>.Get();

        foreach (var kvp in prevCounts)
        {
            if (!currentCounts.ContainsKey(kvp.Key))
                toRemove.Add(kvp.Key);
        }

        for (int i = 0; i < toRemove.Count; i++)
            prevCounts.Remove(toRemove[i]);

        ListPool<string>.Release(toRemove);
    }

    private void TryRefreshSnapshotFromGame()
    {
        prevCounts.Clear();

        if (!Game.IsReady || Game.Ctx == null || Game.Ctx.Inventory == null)
            return;

        var entries = Game.Ctx.Inventory.Data.entries;
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (string.IsNullOrWhiteSpace(e.itemId)) continue;
            prevCounts[e.itemId] = e.count;
        }
    }

    /// <summary>
    /// Tiny pooled-list utility to avoid allocations during removals.
    /// If you don't care, you can delete this and just use a new List<string>().
    /// </summary>
    private static class ListPool<T>
    {
        private static readonly Stack<List<T>> pool = new();

        public static List<T> Get()
        {
            return pool.Count > 0 ? pool.Pop() : new List<T>();
        }

        public static void Release(List<T> list)
        {
            list.Clear();
            pool.Push(list);
        }
    }
}
