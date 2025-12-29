using System.Collections;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class InventoryBarPanelUI : MonoBehaviour
{
    [Header("Panel Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSeconds = 0.20f;
    [SerializeField] private Ease fadeEase = Ease.OutQuad;

    [Header("Slots (size must be 4)")]
    [SerializeField] private InventoryBarSlotUI[] slots = new InventoryBarSlotUI[4];

    private int lastSelectedIndex = -1;
    private bool subscribed;
    
    private struct SlotSnapshot
    {
        public string itemId;
        public int count;
    }
    private readonly SlotSnapshot[] snapshot = new SlotSnapshot[4];


    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Start hidden unless you want it visible in non-level scenes
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        
    }

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        Unsubscribe();
        DOTween.Kill(canvasGroup);
    }

    public void TestPrint()
    {
        Debug.Log("TestPrint");
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (!Game.IsReady || Game.Ctx == null || Game.Ctx.Inventory == null || Game.Ctx.ItemDb == null || Game.Ctx.InventorySelection == null)
            yield return null;

        if (subscribed) yield break;

        Game.Ctx.Inventory.OnChanged += HandleInventoryChanged;
        subscribed = true;

        yield return null; // one frame
        for (int i = 0; i < slots.Length; i++)
            slots[i]?.Rebase();

        RefreshSlots();
        ApplySelection(force: true);

    }

    private void Unsubscribe()
    {
        if (!subscribed) return;

        if (Game.IsReady && Game.Ctx != null && Game.Ctx.Inventory != null)
            Game.Ctx.Inventory.OnChanged -= HandleInventoryChanged;

        subscribed = false;
    }

    private void Update()
    {
        if (!Game.IsReady || Game.Ctx?.InventorySelection == null) return;
        ApplySelection(force: false);
    }

    private void HandleInventoryChanged()
    {
        RefreshSlots();
        ApplySelection(force: true);
    }

    private void RefreshSlots()
    {
        if (!Game.IsReady || Game.Ctx?.Inventory == null || Game.Ctx.ItemDb == null)
            return;

        var entries = Game.Ctx.Inventory.Data.entries;

        for (int i = 0; i < 4; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogError($"InventoryBarPanelUI: slots[{i}] is NULL");
                continue;
            }

            // Desired state for this UI slot
            string desiredItemId = null;
            int desiredCount = 0;

            if (entries != null && i < entries.Count && !string.IsNullOrEmpty(entries[i].itemId))
            {
                desiredItemId = entries[i].itemId;
                desiredCount = entries[i].count;
            }

            // Normalize empties
            if (string.IsNullOrEmpty(desiredItemId))
            {
                desiredItemId = null;
                desiredCount = 0;
            }

            // If unchanged, do nothing (prevents jerk)
            if (snapshot[i].itemId == desiredItemId && snapshot[i].count == desiredCount)
                continue;

            // Update snapshot first
            snapshot[i].itemId = desiredItemId;
            snapshot[i].count = desiredCount;

            // Apply UI change only when needed
            if (desiredItemId == null)
            {
                slots[i].SetEmpty();
            }
            else
            {
                var def = Game.Ctx.ItemDb.Get(desiredItemId);
                if (def == null)
                {
                    Debug.LogWarning($"InventoryBarPanelUI: ItemDatabase missing def for '{desiredItemId}'");
                    slots[i].SetEmpty();
                }
                else
                {
                    slots[i].BindItem(def, desiredCount);
                }
            }
        }
    }


    private void ApplySelection(bool force)
    {
        var entries = Game.Ctx.Inventory.Data.entries;
        int entryCount = entries != null ? entries.Count : 0;

        int selected = (entryCount > 0) ? Game.Ctx.InventorySelection.SelectedIndex : -1;

        if (!force && selected == lastSelectedIndex)
            return;

        // Only update what changed
        if (lastSelectedIndex >= 0 && lastSelectedIndex < 4)
            slots[lastSelectedIndex]?.SetSelected(false);

        if (selected >= 0 && selected < 4 && selected < entryCount)
            slots[selected]?.SetSelected(true);

        lastSelectedIndex = selected;
    }

    // Hook these from GameEventListener responses
    public void FadeInOnLevelStarted()
    {
        FadeTo(1f, true);
    }

    public void FadeOutOnLevelCompleted()
    {
        FadeTo(0f, false);
    }

    private void FadeTo(float alpha, bool interactive)
    {
        DOTween.Kill(canvasGroup);

        canvasGroup.DOFade(alpha, fadeSeconds)
            .SetEase(fadeEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                canvasGroup.interactable = interactive;
                canvasGroup.blocksRaycasts = interactive;
            });
    }
}
