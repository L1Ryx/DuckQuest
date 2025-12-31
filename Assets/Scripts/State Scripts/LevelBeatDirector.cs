using UnityEngine;

public class LevelBeatDirector : MonoBehaviour
{
    [Header("Sequence")]
    [SerializeField] private LevelBeatSequenceSO sequence;

    [Header("Optional Events")]
    [SerializeField] private GameEvent onSequenceStarted;
    [SerializeField] private GameEvent onSequenceCompleted;
    public LevelBeatSequenceSO Sequence => sequence;
    public bool IsRunning => isRunning;


    public int CurrentBeatIndex { get; private set; } = -1;
    public LevelBeatSO CurrentBeat =>
        (sequence != null && CurrentBeatIndex >= 0 && CurrentBeatIndex < sequence.Beats.Count)
            ? sequence.Beats[CurrentBeatIndex]
            : null;

    private bool isRunning;

    private void OnDisable()
    {
        // Safety: unsubscribe if the object is disabled mid-beat.
        UnsubscribeFromAdvanceEvent();
    }

    /// Call this from your LevelStarter when LevelState enters Playing.
    public void StartSequence()
    {
        if (sequence == null || sequence.Beats.Count == 0)
        {
            Debug.LogError("LevelBeatDirector: No beat sequence assigned or sequence is empty.");
            return;
        }

        if (isRunning)
            return;

        isRunning = true;
        CurrentBeatIndex = -1;

        onSequenceStarted?.Raise();
        Advance();
    }

    private void Advance()
    {
        UnsubscribeFromAdvanceEvent();

        CurrentBeatIndex++;

        if (CurrentBeatIndex >= sequence.Beats.Count)
        {
            isRunning = false;
            onSequenceCompleted?.Raise();
            return;
        }

        var beat = CurrentBeat;
        if (beat == null)
            return;

        // Raise all enter events (authored in SO)
        foreach (var evt in beat.OnEnterEvents)
            evt?.Raise();

        // Subscribe to the beat’s advance trigger
        if (beat.AdvanceEvent != null)
            beat.AdvanceEvent.RegisterRuntimeListener(OnAdvanceTriggered);
        else
            Debug.LogWarning($"LevelBeatDirector: Beat '{beat.BeatId}' has no AdvanceEvent.");
    }

    private void OnAdvanceTriggered()
    {
        // Important: advance should be “edge-triggered”.
        // If something raises the advance event multiple times, we only want one transition per beat.
        // Since we unsubscribe on Advance() entry, we are safe.
        Advance();
    }

    private void UnsubscribeFromAdvanceEvent()
    {
        var beat = CurrentBeat;
        if (beat?.AdvanceEvent != null)
            beat.AdvanceEvent.UnregisterRuntimeListener(OnAdvanceTriggered);
    }
}
