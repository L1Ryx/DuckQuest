using UnityEngine;

public class CraftingStation : InventoryCostInteractable
{
    [SerializeField] private CraftingTradeDefinition trade;

    protected override void OnPaymentSucceeded(GameObject interactor)
    {
        if (trade == null || trade.outputItem == null)
        {
            Debug.LogError($"CraftingStation '{name}': Trade output not configured.");
            return;
        }

        var inv = Game.Ctx.Inventory;
        inv.Add(trade.outputItem.itemId, trade.outputCount);

        Debug.Log(
            $"Crafted: -{requiredCount} {requiredItem.displayName}, " +
            $"+{trade.outputCount} {trade.outputItem.displayName}"
        );
    }
}