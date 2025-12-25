using UnityEngine;
using UnityEngine.Events;

public abstract class InventoryCostInteractable : MonoBehaviour, IInteractable
{
    [Header("Inventory Cost")]
    [SerializeField] protected ItemDefinition requiredItem;
    [SerializeField] protected int requiredCount = 1;

    [Header("Usage Rules")]
    [SerializeField] protected bool consumeOnSuccess = true;
    [SerializeField] protected bool oneTimeUse = false;

    [Header("Events (Optional)")]
    [SerializeField] protected UnityEvent onSuccess;
    [SerializeField] protected UnityEvent onFail;

    protected bool hasBeenUsed = false;

    public void Interact(GameObject interactor)
    {
        if (oneTimeUse && hasBeenUsed)
            return;

        if (!Game.IsReady || Game.Ctx.Inventory == null)
        {
            Debug.LogWarning($"{name}: GameContext not ready.");
            return;
        }

        if (requiredItem == null || requiredCount <= 0)
        {
            Debug.LogError($"{name}: Required item not configured.");
            return;
        }

        var inv = Game.Ctx.Inventory;
        string itemId = requiredItem.itemId;

        if (inv.GetCount(itemId) < requiredCount)
        {
            onFail?.Invoke();
            OnPaymentFailed(interactor);
            return;
        }

        if (consumeOnSuccess)
        {
            if (!inv.TryRemove(itemId, requiredCount))
            {
                Debug.LogWarning($"{name}: Failed to remove required item unexpectedly.");
                return;
            }
        }

        hasBeenUsed = true;

        OnPaymentSucceeded(interactor);
        onSuccess?.Invoke();
    }

    /// <summary>
    /// Implement feature-specific success behavior here.
    /// </summary>
    protected abstract void OnPaymentSucceeded(GameObject interactor);

    /// <summary>
    /// Optional override for failure feedback.
    /// </summary>
    protected virtual void OnPaymentFailed(GameObject interactor) { }
}