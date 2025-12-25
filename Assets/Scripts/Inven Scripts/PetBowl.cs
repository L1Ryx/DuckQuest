using UnityEngine;

public class PetBowl : InventoryCostInteractable
{
    protected override void OnPaymentSucceeded(GameObject interactor)
    {
        Debug.Log("You win the level!");
    }
}
