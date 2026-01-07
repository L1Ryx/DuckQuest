using UnityEngine;

/// <summary>
/// Inspector-friendly relay for wiring GameEventListener.Response -> audio actions.
/// Designed for parameterless UnityEvents (matches my current GameEvent system).
/// </summary>
public sealed class AudioEventRelay : MonoBehaviour
{
    public enum RouteMode
    {
        /// <summary>Play/stop on an AudioEmitter on this GameObject.</summary>
        SelfEmitter,

        /// <summary>Play/stop on a specified target emitter (AudioEmitter or GameObject).</summary>
        TargetEmitter,

        /// <summary>Play/stop globally via Game.Ctx.Audio using the GameContext as the emitter.</summary>
        Global
    }

    [Header("Routing")]
    [SerializeField] private RouteMode routeMode = RouteMode.SelfEmitter;

    [Tooltip("Used when Route Mode is TargetEmitter. If AudioEmitter is set, it will be used; otherwise Target GameObject will be used.")]
    [SerializeField] private AudioEmitter targetEmitter;

    [Tooltip("Used when Route Mode is TargetEmitter and Target Emitter is null.")]
    [SerializeField] private GameObject targetGameObject;

    [Header("Cue")]
    [SerializeField] private AudioCue cue;

    [Header("Optional RTPC Set (relay-only convenience)")]
    [Tooltip("Optional RTPC name to set when calling SetRtpc().")]
    [SerializeField] private string rtpcName;

    [Tooltip("Optional RTPC value to set when calling SetRtpc().")]
    [SerializeField] private float rtpcValue;

    [Tooltip("If true, SetRtpc() applies globally; otherwise it applies to the routed emitter.")]
    [SerializeField] private bool rtpcIsGlobal = true;

    /// <summary>Set the configured cue's Play event using the configured routing.</summary>
    public void Play()
    {
        if (cue == null)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' has no AudioCue assigned.", this);
            return;
        }

        switch (routeMode)
        {
            case RouteMode.SelfEmitter:
                PlayOnSelf();
                break;

            case RouteMode.TargetEmitter:
                PlayOnTarget();
                break;

            case RouteMode.Global:
                PlayGlobal();
                break;
        }
    }

    /// <summary>Post the configured cue's Stop event (if any) using the configured routing.</summary>
    public void Stop()
    {
        if (cue == null)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' has no AudioCue assigned.", this);
            return;
        }

        switch (routeMode)
        {
            case RouteMode.SelfEmitter:
                StopOnSelf();
                break;

            case RouteMode.TargetEmitter:
                StopOnTarget();
                break;

            case RouteMode.Global:
                StopGlobal();
                break;
        }
    }

    /// <summary>
    /// Convenience method for parameterless events: set an RTPC to the configured value.
    /// Useful for quick wiring until you add FloatGameEvent / FloatGameEventListener.
    /// </summary>
    public void SetRtpc()
    {
        if (string.IsNullOrWhiteSpace(rtpcName))
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' has no RTPC name configured.", this);
            return;
        }

        if (!Game.IsReady)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' cannot set RTPC because GameContext is not ready.", this);
            return;
        }

        if (rtpcIsGlobal)
        {
            Game.Ctx.Audio.SetGlobalRtpc(rtpcName, rtpcValue);
            return;
        }

        // Per-emitter RTPC: apply to routed emitter.
        var emitterGO = ResolveEmitterGameObject();
        if (emitterGO == null)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' could not resolve an emitter GameObject for per-emitter RTPC.", this);
            return;
        }

        Game.Ctx.Audio.SetRtpcOn(rtpcName, rtpcValue, emitterGO);
    }

    // -----------------------
    // Internal implementations
    // -----------------------

    private void PlayOnSelf()
    {
        var emitter = GetComponent<AudioEmitter>();
        if (emitter == null)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' is set to SelfEmitter but no AudioEmitter component was found.", this);
            return;
        }

        emitter.Play(cue);
    }

    private void StopOnSelf()
    {
        var emitter = GetComponent<AudioEmitter>();
        if (emitter == null)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' is set to SelfEmitter but no AudioEmitter component was found.", this);
            return;
        }

        emitter.Stop(cue);
    }

    private void PlayOnTarget()
    {
        if (targetEmitter != null)
        {
            targetEmitter.Play(cue);
            return;
        }

        if (targetGameObject == null)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' is set to TargetEmitter but no target is assigned.", this);
            return;
        }

        if (!Game.IsReady)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' cannot route to target because GameContext is not ready.", this);
            return;
        }

        Game.Ctx.Audio.PlayCueOn(cue, targetGameObject);
    }

    private void StopOnTarget()
    {
        if (targetEmitter != null)
        {
            targetEmitter.Stop(cue);
            return;
        }

        if (targetGameObject == null)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' is set to TargetEmitter but no target is assigned.", this);
            return;
        }

        if (!Game.IsReady)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' cannot route to target because GameContext is not ready.", this);
            return;
        }

        Game.Ctx.Audio.StopCueOn(cue, targetGameObject);
    }

    private void PlayGlobal()
    {
        if (!Game.IsReady)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' cannot play globally because GameContext is not ready.", this);
            return;
        }

        Game.Ctx.Audio.PlayCueGlobal(cue);
    }

    private void StopGlobal()
    {
        if (!Game.IsReady)
        {
            Debug.LogWarning($"{nameof(AudioEventRelay)} on '{name}' cannot stop globally because GameContext is not ready.", this);
            return;
        }

        Game.Ctx.Audio.StopCueGlobal(cue);
    }

    private GameObject ResolveEmitterGameObject()
    {
        switch (routeMode)
        {
            case RouteMode.SelfEmitter:
                return gameObject;

            case RouteMode.TargetEmitter:
                if (targetEmitter != null) return targetEmitter.gameObject;
                return targetGameObject;

            case RouteMode.Global:
                // For per-emitter RTPC, global routing isn't meaningful. Return global emitter anyway.
                return Game.IsReady ? Game.Ctx.gameObject : null;

            default:
                return null;
        }
    }
}
