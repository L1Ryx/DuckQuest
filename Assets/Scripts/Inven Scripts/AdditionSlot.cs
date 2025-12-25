using UnityEngine;

public class AdditionSlot : MonoBehaviour, IInteractable
{
    [Header("Visual (prototype for later)")]
    [SerializeField] private SpriteRenderer filledSpriteRenderer;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite filledSprite;

    public bool HasValue { get; private set; }
    public int Value { get; private set; }
    public string StoredItemId { get; private set; } // for refund

    public void Interact(GameObject interactor)
    {
        if (!Game.IsReady || Game.Ctx.Inventory == null || Game.Ctx.ItemDb == null || Game.Ctx.InventorySelection == null)
        {
            Debug.LogWarning($"{name}: Missing GameContext dependencies.");
            return;
        }

        var inv = Game.Ctx.Inventory;

        // If filled, clicking removes it (refund) for prototype convenience
        if (HasValue)
        {
            // Refund back to inventory (may fail if inventory is full of types)
            bool refunded = inv.TryAdd(StoredItemId, 1);
            if (!refunded)
            {
                Debug.Log($"{name}: Cannot refund {StoredItemId} because backpack is full (4 types).");
                return;
            }

            ClearNoRefund(); // state clear only; refund already done
            UpdateVisual();
            return;
        }

        // Empty slot: place currently selected item (must be a hardworm pack)
        string selectedItemId = Game.Ctx.InventorySelection.GetSelectedItemId();
        if (string.IsNullOrEmpty(selectedItemId))
            return;

        var def = Game.Ctx.ItemDb.Get(selectedItemId);
        if (def is not HardwormPackDefinition hwDef)
        {
            Debug.Log($"{name}: Selected item is not a hardworm pack.");
            return;
        }

        // Must have at least 1 pack
        if (inv.GetCount(selectedItemId) < 1)
        {
            Debug.Log($"{name}: Not enough of selected item to place.");
            return;
        }

        // Consume 1 pack
        if (!inv.TryRemove(selectedItemId, 1))
        {
            Debug.LogWarning($"{name}: Failed to remove selected item unexpectedly.");
            return;
        }

        HasValue = true;
        Value = hwDef.packSize;
        StoredItemId = selectedItemId;

        UpdateVisual();
    }

    public void ClearNoRefund()
    {
        HasValue = false;
        Value = 0;
        StoredItemId = null;
        UpdateVisual();
    }

    private void Reset()
    {
        filledSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void UpdateVisual()
    {
        if (filledSpriteRenderer == null) return;

        if (!HasValue)
        {
            if (emptySprite != null) filledSpriteRenderer.sprite = emptySprite;
        }
        else
        {
            if (filledSprite != null) filledSpriteRenderer.sprite = filledSprite;
        }
    }
}
