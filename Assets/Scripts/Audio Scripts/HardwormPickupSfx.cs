using System.Collections;
using UnityEngine;

public sealed class HardwormPickupSfx : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioCue pickupCue;

    [Header("Multi-play")]
    [Tooltip("Delay between each pickup tick sound when playing multiple ticks.")]
    [Min(0f)]
    [SerializeField] private float delayBetweenPlays = 0.06f;

    [Tooltip("If true, a new pickup request cancels any currently playing tick sequence.")]
    [SerializeField] private bool cancelPreviousSequence = true;

    private Coroutine _activeRoutine;

    public void PlayPickup(HardwormPackDefinition packDef)
    {
        int plays = packDef != null ? Mathf.Max(1, packDef.packSize) : 1;
        PlayPickupCount(plays);
    }

    public void PlayPickupCount(int plays)
    {
        if (pickupCue == null || !Game.IsReady)
            return;

        plays = Mathf.Max(1, plays);

        if (cancelPreviousSequence && _activeRoutine != null)
        {
            StopCoroutine(_activeRoutine);
            _activeRoutine = null;
        }

        _activeRoutine = StartCoroutine(PlaySequence(plays));
    }

    private IEnumerator PlaySequence(int plays)
    {
        // First tick immediately for responsiveness
        Game.Ctx.Audio.PlayCueGlobal(pickupCue);

        for (int i = 1; i < plays; i++)
        {
            if (delayBetweenPlays > 0f)
                yield return new WaitForSecondsRealtime(delayBetweenPlays);

            Game.Ctx.Audio.PlayCueGlobal(pickupCue);
        }

        _activeRoutine = null;
    }
}