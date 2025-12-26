using System.Collections;
using UnityEngine;

public class LevelStateGameEventBridge : MonoBehaviour
{
    [Header("Raised from LevelStateModel")]
    [SerializeField] private GameEvent onLevelStarted;
    [SerializeField] private GameEvent onLevelCompleted;

    private bool subscribed;

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private IEnumerator SubscribeWhenReady()
    {
        // Wait for bootstrap to finish creating the runtime model.
        while (!Game.IsReady || Game.Ctx == null || Game.Ctx.LevelState == null)
            yield return null;

        Subscribe();
    }

    private void Subscribe()
    {
        if (subscribed) return;

        Game.Ctx.LevelState.OnLevelStarted += HandleLevelStarted;
        Game.Ctx.LevelState.OnLevelCompleted += HandleLevelCompleted;

        subscribed = true;

        // Optional: if LevelStarter already began the level before we subscribed,
        // you may want to "catch up" here. With your LevelStateModel, simplest is:
        if (Game.Ctx.LevelState.CurrentState == LevelState.Playing)
        {
            HandleLevelStarted();
        }
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;

        if (Game.IsReady && Game.Ctx != null && Game.Ctx.LevelState != null)
        {
            Game.Ctx.LevelState.OnLevelStarted -= HandleLevelStarted;
            Game.Ctx.LevelState.OnLevelCompleted -= HandleLevelCompleted;
        }

        subscribed = false;
    }

    private void HandleLevelStarted()
    {
        if (onLevelStarted != null)
            onLevelStarted.Raise();
    }

    private void HandleLevelCompleted()
    {
        if (onLevelCompleted != null)
            onLevelCompleted.Raise();
    }
}