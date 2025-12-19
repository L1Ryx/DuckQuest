using UnityEngine;

public class HardwormPickup : MonoBehaviour, IInteractable
{
    [Header("Definition")]
    [SerializeField] private HardwormPackDefinition packDef;

    [Header("Pickup Amount")]
    [Tooltip("Number of packs granted (usually 1).")]
    [SerializeField] private int packsGranted = 1;

    [Header("Events")]
    [SerializeField] private GameEvent onPickedUp; // optional: hook to your SO event system

    public void Interact(GameObject interactor)
    {
        if (!Game.IsReady || Game.Ctx.Inventory == null)
        {
            Debug.LogWarning("Cannot pick up hardworm: GameContext/Inventory not ready. Start from Bootstrap scene.");
            return;
        }

        if (packDef == null || string.IsNullOrWhiteSpace(packDef.itemId))
        {
            Debug.LogError($"HardwormPickup on '{name}' is missing a valid HardwormPackDefinition (itemId).");
            return;
        }

        if (packsGranted <= 0) packsGranted = 1;

        Game.Ctx.Inventory.Add(packDef.itemId, packsGranted);

        if (onPickedUp != null) onPickedUp.Raise();

        Destroy(gameObject);
    }
}