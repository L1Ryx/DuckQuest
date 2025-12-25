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

    // Optional: if you want “take back” behavior later, slots already support it.
}