using UnityEngine;

public class PetBowl : InventoryCostInteractable
{
    protected override void OnPaymentSucceeded(GameObject interactor)
    {
        Game.Ctx.LevelState.CompleteLevel();
    }
}
