using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AdditionSlotHoverPanel : MonoBehaviour, IHoverInfoUI
{
    [Header("Spawn")]
    [SerializeField] private GameObject screenSpacePanelPrefab;
    [SerializeField] private Transform worldAnchor;
    [SerializeField] private string worldUIRootTag = "WorldUIRoot";

    [Header("Copy")]
    [SerializeField] private string title = "Ancient Slot";
    [TextArea] [SerializeField] private string description = "Combine hardworms.";

    [Header("Empty State")]
    [SerializeField] private Sprite emptyIcon;
    [SerializeField] private string emptyName = "Empty";

    [Header("Symbol")]
    [SerializeField] private Sprite symbolActive;
    [SerializeField] private Sprite symbolInactive;

    [Header("Tween")]
    [SerializeField] private float showDuration = 0.18f;
    [SerializeField] private float hideDuration = 0.12f;
    [SerializeField] private float hiddenScale = 0.88f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InCubic;

    [Header("Alpha")]
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.45f;

    private GameObject panelInstance;
    private AdditionSlotHoverPanelView view;

    private AdditionSlot slot;
    private Tween activeTween;
    private bool isVisible;

    // Track last hover inputs so Refresh() can recompute correctly
    private bool lastHovered;
    private bool lastInRange;

    private void Awake()
    {
        if (worldAnchor == null) worldAnchor = transform;

        slot = GetComponent<AdditionSlot>();
        if (slot == null)
        {
            Debug.LogError($"{nameof(AdditionSlotHoverPanel)} requires AdditionSlot on the same GameObject.", this);
            enabled = false;
            return;
        }

        SpawnPanel();
        if (panelInstance == null) return;

        // Static copy
        view.titleText.text = title;
        view.descText.text = description;

        // Initial content
        ApplyStoredItemUI();

        // Start hidden
        panelInstance.SetActive(true);
        view.canvasGroup.alpha = 0f;
        view.panelTransform.localScale = Vector3.one * hiddenScale;
        isVisible = false;

        // Initial symbol
        ApplyAccessibilityVisuals(isActive: false);
    }

    private void OnDestroy()
    {
        if (panelInstance != null)
            Destroy(panelInstance);
    }

    private void SpawnPanel()
    {
        if (screenSpacePanelPrefab == null)
        {
            Debug.LogError($"{nameof(AdditionSlotHoverPanel)}: screenSpacePanelPrefab not assigned.", this);
            enabled = false;
            return;
        }

        GameObject rootGO = GameObject.FindGameObjectWithTag(worldUIRootTag);
        if (rootGO == null)
        {
            Debug.LogError($"{nameof(AdditionSlotHoverPanel)}: Could not find WorldUIRoot with tag '{worldUIRootTag}'.", this);
            enabled = false;
            return;
        }

        var canvas = rootGO.GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError($"{nameof(AdditionSlotHoverPanel)}: WorldUIRoot has no Canvas.", this);
            enabled = false;
            return;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        panelInstance = Instantiate(screenSpacePanelPrefab, canvasRect);

        view = panelInstance.GetComponent<AdditionSlotHoverPanelView>();
        if (view == null || view.panelTransform == null || view.canvasGroup == null ||
            view.titleText == null || view.descText == null ||
            view.itemIcon == null || view.itemName == null)
        {
            Debug.LogError($"{nameof(AdditionSlotHoverPanel)}: Prefab missing AdditionSlotHoverPanelView refs.", this);
            enabled = false;
            return;
        }

        var follow = panelInstance.GetComponent<ScreenSpaceFollowWorld>();
        if (follow == null) follow = panelInstance.AddComponent<ScreenSpaceFollowWorld>();
        follow.Init(Camera.main, canvasRect, worldAnchor);
    }

    public void SetHoverState(bool isHovered, bool inRange)
    {
        if (!enabled) return;

        lastHovered = isHovered;
        lastInRange = inRange;

        if (isHovered) Show(inRange);
        else Hide();
    }

    /// <summary>
    /// Call this from a GameEventListener (slot state changed, inventory changed, selection changed, etc.)
    /// to refresh content + accessibility while hovered/visible.
    /// </summary>
    public void Refresh()
    {
        if (!enabled) return;

        ApplyStoredItemUI();

        if (isVisible && lastHovered)
        {
            bool active = lastInRange && CanInteractNow();
            ApplyAccessibilityVisuals(active);
        }
    }

    private void Show(bool inRange)
    {
        // Always update content on show (in case slot changed while not hovered)
        ApplyStoredItemUI();

        bool active = inRange && CanInteractNow();
        ApplyAccessibilityVisuals(active);

        if (!isVisible)
        {
            isVisible = true;
            view.canvasGroup.alpha = 0f;
            view.panelTransform.localScale = Vector3.one * hiddenScale;
        }

        activeTween?.Kill();

        activeTween = DOTween.Sequence()
            .Join(view.canvasGroup.DOFade(active ? activeAlpha : inactiveAlpha, 0.12f))
            .Join(view.panelTransform.DOScale(1f, showDuration).SetEase(showEase))
            .SetUpdate(true);
    }

    private void Hide()
    {
        if (!isVisible) return;

        isVisible = false;
        activeTween?.Kill();

        activeTween = DOTween.Sequence()
            .Join(view.canvasGroup.DOFade(0f, hideDuration))
            .Join(view.panelTransform.DOScale(hiddenScale, hideDuration).SetEase(hideEase))
            .SetUpdate(true);
    }

    private void ApplyStoredItemUI()
    {
        // Slot filled: show stored item info from ItemDb.
        if (slot.HasValue && !string.IsNullOrEmpty(slot.StoredItemId) && Game.IsReady && Game.Ctx.ItemDb != null)
        {
            var def = Game.Ctx.ItemDb.Get(slot.StoredItemId);
            if (def != null)
            {
                view.itemName.text = def.displayName;
                view.itemIcon.sprite = def.icon;
                view.itemIcon.enabled = def.icon != null;
                return;
            }
        }

        // Empty (or unknown): show empty state
        view.itemName.text = emptyName;
        view.itemIcon.sprite = emptyIcon;
        view.itemIcon.enabled = emptyIcon != null;
    }

    private bool CanInteractNow()
    {
        if (!Game.IsReady || Game.Ctx.Inventory == null || Game.Ctx.ItemDb == null || Game.Ctx.InventorySelection == null)
            return false;

        var inv = Game.Ctx.Inventory;

        // If filled: can take out only if refund is possible (TryAdd would succeed)
        if (slot.HasValue && !string.IsNullOrEmpty(slot.StoredItemId))
        {
            // If you don't have a CanAdd API, safest is to attempt a "dry run" pattern:
            // Here we approximate: if inventory is limited by types, you may need a real CanAdd method.
            // If you *do* have CanAdd, use that instead.
            return inv.CanAdd(slot.StoredItemId); // <-- if this doesn't exist, tell me and I'll adapt to your InventoryModel
        }

        // If empty: can place selected hardworm pack if available
        string selectedItemId = Game.Ctx.InventorySelection.GetSelectedItemId();
        if (string.IsNullOrEmpty(selectedItemId))
            return false;

        var def = Game.Ctx.ItemDb.Get(selectedItemId);
        if (def is not HardwormPackDefinition)
            return false;

        return inv.GetCount(selectedItemId) >= 1;
    }

    private void ApplyAccessibilityVisuals(bool isActive)
    {
        if (view.symbolImage != null)
            view.symbolImage.sprite = isActive ? symbolActive : symbolInactive;

        // Do not hard-set alpha here; Show() handles fade to correct alpha.
        // Refresh() may call this; in that case we tween alpha lightly:
        if (isVisible)
        {
            float targetAlpha = isActive ? activeAlpha : inactiveAlpha;
            view.canvasGroup.DOKill();
            view.canvasGroup.DOFade(targetAlpha, 0.08f).SetUpdate(true);
        }
    }
}
