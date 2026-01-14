using UnityEngine;

public class AdditionMachine : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private AdditionSlot slotA;
    [SerializeField] private AdditionSlot slotB;

    [Header("Mode")]
    [Tooltip("If true, machine computes A - B. If false, computes A + B.")]
    [SerializeField] private bool isSubtractionMachine = false;

    [Tooltip("Optional clarity flag. Keep this true unless you intentionally want a subtraction machine.")]
    [SerializeField] private bool isAdditionMachine = true;

    public bool HasBothInputs => slotA != null && slotB != null && slotA.HasValue && slotB.HasValue;

    public bool IsSubtractionMachine => isSubtractionMachine;
    public bool IsAdditionMachine => isAdditionMachine && !isSubtractionMachine;

    public int Sum => (slotA != null ? slotA.Value : 0) + (slotB != null ? slotB.Value : 0);

    public int Result
    {
        get
        {
            
            int a = (slotA != null ? slotA.Value : 0);
            int b = (slotB != null ? slotB.Value : 0);

            Debug.Log($"[{name}] sub={isSubtractionMachine} add={isAdditionMachine} a={a} b={b} => {(isSubtractionMachine ? a-b : a+b)}");

            // Subtraction takes precedence if enabled.
            if (isSubtractionMachine)
                return a - b;

            return a + b;
        }
    }

    public bool HasValidResult => HasBothInputs && Result > 0;

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

    private void OnValidate()
    {
        // If subtraction is enabled, "addition" should not also be true.
        // This avoids ambiguous inspector states.
        if (isSubtractionMachine)
            isAdditionMachine = false;

        // If neither is checked, default back to addition to prevent a "dead" machine.
        if (!isSubtractionMachine && !isAdditionMachine)
            isAdditionMachine = true;
    }

    // Optional: if you want “take back” behavior later, slots already support it.
}
