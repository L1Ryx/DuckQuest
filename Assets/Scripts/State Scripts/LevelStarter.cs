using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    private void Start()
    {
        if (!Game.IsReady || Game.Ctx.LevelState == null)
        {
            Debug.LogWarning("LevelStarter: GameContext not ready.");
            return;
        }

        Game.Ctx.LevelState.BeginLevel();
    }
}