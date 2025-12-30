using UnityEngine;

public class InteractionLockEventBridge : MonoBehaviour
{
    [Header("Events In")]
    [SerializeField] private GameEvent lockInteractions;
    [SerializeField] private GameEvent unlockInteractions;

    private void OnEnable()
    {
        // You already use GameEventListener, so just put two listeners on this GameObject
        // OR implement listener logic here. Keeping it consistent:
    }

    // These are UnityEvent-callable functions for GameEventListener.Response
    public void AcquireLock()
    {
        if (!Game.IsReady || Game.Ctx?.InteractionLock == null) return;
        Game.Ctx.InteractionLock.Acquire();
    }

    public void ReleaseLock()
    {
        if (!Game.IsReady || Game.Ctx?.InteractionLock == null) return;
        Game.Ctx.InteractionLock.Release();
    }

    public void ForceClearLocks()
    {
        if (!Game.IsReady || Game.Ctx?.InteractionLock == null) return;
        Game.Ctx.InteractionLock.ForceClear();
    }
}