using UnityEngine;
using UnityEngine.Events;

public class AdditionOutputSlot : MonoBehaviour, IInteractable
{
    [SerializeField] private AdditionMachine machine;

    [Header("Rules")]
    [SerializeField] private bool clearInputsAfterSuccess = true;

    public AdditionMachine Machine => machine;
    [Header("Events")] 
    [SerializeField] private UnityEvent onOutputSlotChanged;

    public void Interact(GameObject interactor)
    {
        if (!Game.IsReady || Game.Ctx.Inventory == null || Game.Ctx.ItemDb == null)
            return;

        if (machine == null)
        {
            Debug.LogError($"{name}: Output slot missing machine reference.");
            return;
        }

        if (!machine.HasBothInputs)
        {
            Debug.Log($"{name}: Need both inputs filled.");
            return;
        }

        int sum = machine.Sum;
        if (sum <= 0)
            return;

        // Convert sum -> a hardworm pack definition
        var outDef = Game.Ctx.ItemDb.GetHardwormByPackSize(sum);
        if (outDef == null)
        {
            Debug.Log($"{name}: No hardworm pack definition exists for sum={sum}.");
            return;
        }

        bool added = Game.Ctx.Inventory.TryAdd(outDef.itemId, 1);
        if (!added)
        {
            Debug.Log($"{name}: Backpack is full (4 types). Cannot add result '{outDef.itemId}'.");
            return;
        }

        if (clearInputsAfterSuccess)
            machine.ClearInputs();

        onOutputSlotChanged?.Invoke();

        Debug.Log($"Addition result: {sum} hardworms -> +1 {outDef.displayName}");
    }
}