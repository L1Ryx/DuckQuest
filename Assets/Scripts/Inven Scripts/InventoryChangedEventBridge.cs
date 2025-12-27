using UnityEngine;

public class InventoryChangedEventBridge : MonoBehaviour
{
    [SerializeField] private GameEvent onInventoryChanged;

    private void OnEnable()
    {
        // Game.Ctx might not be ready the same frame this enables, so we do a safe subscribe.
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (!Game.IsReady || Game.Ctx == null || Game.Ctx.Inventory == null)
            return;

        Game.Ctx.Inventory.OnChanged += HandleInventoryChanged;
    }

    private void Unsubscribe()
    {
        if (!Game.IsReady || Game.Ctx == null || Game.Ctx.Inventory == null)
            return;

        Game.Ctx.Inventory.OnChanged -= HandleInventoryChanged;
    }

    private void HandleInventoryChanged()
    {
        onInventoryChanged?.Raise();
    }
}