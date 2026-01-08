using UnityEngine;

public class AdditionMachine : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private AdditionSlot slotA;
    [SerializeField] private AdditionSlot slotB;

    public bool HasBothInputs => slotA != null && slotB != null && slotA.HasValue && slotB.HasValue;

    public int Sum => (slotA != null ? slotA.Value : 0) + (slotB != null ? slotB.Value : 0);

    public void ClearInputs()
    {
        slotA?.ClearNoRefund();
        slotB?.ClearNoRefund();
    }
    
    public bool WereBothInputsEmptyBeforePlacing(AdditionSlot placingSlot)
    {
        // Called from AdditionSlot BEFORE it sets HasValue=true.
        // If the "other" slot is empty right now, then both were empty.
        if (slotA == null || slotB == null) return true;

        if (placingSlot == slotA)
            return !slotB.HasValue;

        if (placingSlot == slotB)
            return !slotA.HasValue;

        // If something unexpected calls this, treat as first placement.
        return true;
    }


    // Optional: if you want “take back” behavior later, slots already support it.
}