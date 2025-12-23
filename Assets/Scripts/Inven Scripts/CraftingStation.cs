using UnityEngine;

public class CraftingStation : MonoBehaviour, IInteractable
{
    [SerializeField] private CraftingTradeDefinition trade;

    public void Interact(GameObject interactor)
    {
        if (!Game.IsReady || Game.Ctx.Inventory == null)
        {
            Debug.LogWarning("CraftingStation: GameContext not ready. Start from Bootstrap.");
            return;
        }

        if (trade == null || trade.inputItem == null || trade.outputItem == null)
        {
            Debug.LogError($"CraftingStation '{name}': trade definition not assigned properly.");
            return;
        }

        var inv = Game.Ctx.Inventory;

        string inId = trade.inputItem.itemId;
        string outId = trade.outputItem.itemId;

        // Start simple: must have enough input packs/items.
        if (inv.GetCount(inId) < trade.inputCount)
        {
            Debug.Log($"CraftingStation '{name}': not enough {trade.inputItem.displayName}.");
            return;
        }

        if (!inv.TryRemove(inId, trade.inputCount))
        {
            Debug.LogWarning($"CraftingStation '{name}': remove failed unexpectedly.");
            return;
        }

        inv.Add(outId, trade.outputCount);

        Debug.Log($"Crafted: -{trade.inputCount} {trade.inputItem.displayName}, +{trade.outputCount} {trade.outputItem.displayName}");
    }
}