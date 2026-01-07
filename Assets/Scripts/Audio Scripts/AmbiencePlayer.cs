using System.Collections;
using UnityEngine;

/// <summary>
/// Prefab-friendly controller for global ambience.
/// Exposes Play/Stop methods intended to be called from GameEventListener UnityEvents.
/// </summary>
public sealed class AmbiencePlayer : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private AudioCue ambienceCue;

    [SerializeField] private float startDelay = 0.05f;

    [Header("Behavior")]
    [Tooltip("If true, Stop() is called automatically when this object is disabled/destroyed.")]
    [SerializeField] private bool stopOnDisable = true;

    [Tooltip("If true, Play() is called automatically on Start().")]
    [SerializeField] private bool playOnStart = false;

    private bool _hasRequestedPlay;

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    private void OnDisable()
    {
        if (!stopOnDisable)
            return;

        // Only stop if we previously started (prevents accidental Stop calls).
        if (_hasRequestedPlay)
            Stop();
    }

    /// <summary>
    /// Starts this ambience as the global ambience via AudioStateModel ownership.
    /// Safe to call multiple times (AudioStateModel should be idempotent).
    /// </summary>
    public void Play() {
        
        _hasRequestedPlay = true;

        StartCoroutine(PlayWhenReady());
        
    }

    IEnumerator PlayWhenReady()
    {
        yield return new WaitForSeconds(startDelay);
        if (!Game.IsReady)
        {
            Debug.LogWarning($"{nameof(AmbiencePlayer)}: GameContext not ready. Cannot play ambience.", this);
            yield break;
        }

        if (ambienceCue == null || !ambienceCue.HasPlayEvent)
        {
            Debug.LogWarning($"{nameof(AmbiencePlayer)}: No valid AudioCue assigned.", this);
            yield break;
        }
        Game.Ctx.Audio.SetGlobalAmbience(ambienceCue);
    }

    /// <summary>
    /// Stops the currently owned global ambience via AudioStateModel.
    /// </summary>
    public void Stop()
    {
        if (!Game.IsReady)
        {
            Debug.LogWarning($"{nameof(AmbiencePlayer)}: GameContext not ready. Cannot stop ambience.", this);
            return;
        }

        Game.Ctx.Audio.StopGlobalAmbience(immediate: false);
        _hasRequestedPlay = false;
    }
}
