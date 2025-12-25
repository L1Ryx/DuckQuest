using UnityEngine;
using System;

public enum LevelState
{
    NotStarted,
    Playing,
    Completed
}

public class LevelStateModel
{
    public LevelState CurrentState { get; private set; } = LevelState.NotStarted;

    public event Action<LevelState, LevelState> OnStateChanged;
    public event Action OnLevelStarted;
    public event Action OnLevelCompleted;

    public void BeginLevel()
    {
        if (CurrentState != LevelState.NotStarted)
            return;

        SetState(LevelState.Playing);
        OnLevelStarted?.Invoke();
    }

    public void CompleteLevel()
    {
        if (CurrentState != LevelState.Playing)
            return;

        SetState(LevelState.Completed);
        OnLevelCompleted?.Invoke();
    }

    private void SetState(LevelState next)
    {
        var prev = CurrentState;
        CurrentState = next;
        OnStateChanged?.Invoke(prev, next);
    }
}