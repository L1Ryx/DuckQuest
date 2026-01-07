using System;
using UnityEngine;

/// <summary>
/// Context-owned audio state + policy.
/// Owns looping music / ambience decisions and exposes RTPC helpers.
/// 
/// Assumes Wwise Unity Integration is installed (AkSoundEngine available).
/// </summary>
public sealed class AudioStateModel
{
    private GameObject _globalEmitter;

    // Track "current music" to prevent accidental restarts.
    private string _currentMusicEventName;
    private uint _currentMusicPlayingId;

    // Track paused state if you want to enforce policy.
    private bool _isPaused;

    /// <summary>
    /// Initialize with a GameObject that represents your global 2D emitter.
    /// A good default is the GameContext GameObject (DontDestroyOnLoad).
    /// </summary>
    public void Initialize(GameObject globalEmitter)
    {
        if (globalEmitter == null)
            throw new ArgumentNullException(nameof(globalEmitter));

        _globalEmitter = globalEmitter;
    }

    /// <summary>
    /// Posts a one-shot (or loop) event globally (2D-ish, or at least at the global emitter location).
    /// </summary>
    public uint PostGlobal(string wwiseEventName)
    {
        EnsureInitialized();
        if (string.IsNullOrWhiteSpace(wwiseEventName))
            return 0;

        return AkSoundEngine.PostEvent(wwiseEventName, _globalEmitter);
    }

    /// <summary>
    /// Posts an event on a specific emitter GameObject (3D positional).
    /// </summary>
    public uint PostOn(string wwiseEventName, GameObject emitter)
    {
        EnsureInitialized();
        if (string.IsNullOrWhiteSpace(wwiseEventName) || emitter == null)
            return 0;

        return AkSoundEngine.PostEvent(wwiseEventName, emitter);
    }

    // -------------------------
    // Music ownership (looping)
    // -------------------------

    /// <summary>
    /// Sets (starts) music by posting the event name on the global emitter.
    /// If the same music is already playing, it does nothing (idempotent).
    ///
    /// If you pass a different event name, it will stop the previous playing ID (if any)
    /// and start the new one. For crossfades, you typically want Wwise Music Switch/States
    /// or dedicated stop events; this is a minimal "ownership" baseline.
    /// </summary>
    public void SetMusic(string musicEventName)
    {
        EnsureInitialized();

        if (string.IsNullOrWhiteSpace(musicEventName))
            return;

        // Avoid restarting the same music.
        if (string.Equals(_currentMusicEventName, musicEventName, StringComparison.Ordinal) &&
            _currentMusicPlayingId != 0)
        {
            return;
        }

        // Stop previous track if we own a playing ID.
        StopMusic();

        _currentMusicEventName = musicEventName;
        _currentMusicPlayingId = AkSoundEngine.PostEvent(musicEventName, _globalEmitter);
    }

    /// <summary>
    /// Stops the currently owned music playing ID, if any.
    /// </summary>
    public void StopMusic()
    {
        if (_currentMusicPlayingId != 0)
        {
            AkSoundEngine.StopPlayingID(_currentMusicPlayingId);
            _currentMusicPlayingId = 0;
        }

        _currentMusicEventName = null;
    }

    // -------------------------
    // RTPC helpers
    // -------------------------

    /// <summary>
    /// Set a global RTPC value (applies project-wide).
    /// </summary>
    public void SetGlobalRtpc(string rtpcName, float value)
    {
        EnsureInitialized();
        if (string.IsNullOrWhiteSpace(rtpcName))
            return;

        // Global RTPC: no emitter needed
        AkSoundEngine.SetRTPCValue(rtpcName, value);
    }

    /// <summary>
    /// Set an RTPC value scoped to a specific emitter.
    /// Useful for per-object parameters (e.g., engine RPM, proximity intensity, etc.).
    /// </summary>
    public void SetRtpcOn(string rtpcName, float value, GameObject emitter)
    {
        EnsureInitialized();
        if (string.IsNullOrWhiteSpace(rtpcName) || emitter == null)
            return;

        AkSoundEngine.SetRTPCValue(rtpcName, value, emitter);
    }

    // -------------------------
    // Optional: Pause policy
    // -------------------------

    public void SetPaused(bool paused)
    {
        EnsureInitialized();

        if (_isPaused == paused)
            return;

        _isPaused = paused;

        // This is a placeholder policy.
        // Many teams prefer a Wwise State (e.g., "GameState=Paused") or a global RTPC (e.g., "PauseLowpass").
        // Uncomment if you have a pause state in Wwise:
        // AkSoundEngine.SetState("GameState", paused ? "Paused" : "Unpaused");
    }
    
        /// <summary>
    /// Plays a cue globally (posts play event and applies any RTPC bindings).
    /// </summary>
    public uint PlayCueGlobal(AudioCue cue)
    {
        EnsureInitialized();
        if (cue == null || !cue.HasPlayEvent)
            return 0;

        ApplyCueRtpcs(cue, emitter: null);
        return AkSoundEngine.PostEvent(cue.playEvent, _globalEmitter);
    }

    /// <summary>
    /// Plays a cue on a specific emitter (posts play event and applies any RTPC bindings).
    /// </summary>
    public uint PlayCueOn(AudioCue cue, GameObject emitter)
    {
        EnsureInitialized();
        if (cue == null || !cue.HasPlayEvent || emitter == null)
            return 0;

        ApplyCueRtpcs(cue, emitter);
        return AkSoundEngine.PostEvent(cue.playEvent, emitter);
    }

    /// <summary>
    /// Stops a cue globally, if it has a stop event.
    /// </summary>
    public void StopCueGlobal(AudioCue cue)
    {
        EnsureInitialized();
        if (cue == null || !cue.HasStopEvent)
            return;

        AkSoundEngine.PostEvent(cue.stopEvent, _globalEmitter);
    }

    /// <summary>
    /// Stops a cue on a specific emitter, if it has a stop event.
    /// </summary>
    public void StopCueOn(AudioCue cue, GameObject emitter)
    {
        EnsureInitialized();
        if (cue == null || !cue.HasStopEvent || emitter == null)
            return;

        AkSoundEngine.PostEvent(cue.stopEvent, emitter);
    }

    private void ApplyCueRtpcs(AudioCue cue, GameObject emitter)
    {
        if (cue == null || cue.rtpcBindings == null)
            return;

        for (int i = 0; i < cue.rtpcBindings.Length; i++)
        {
            var binding = cue.rtpcBindings[i];

            // Validate binding
            if (binding.rtpc == null || !binding.rtpc.IsValid)
                continue;

            float v = binding.rtpc.ClampValue(binding.value);

            if (binding.isGlobal)
            {
                AkSoundEngine.SetRTPCValue(binding.rtpc.rtpcName, v);
            }
            else if (emitter != null)
            {
                AkSoundEngine.SetRTPCValue(binding.rtpc.rtpcName, v, emitter);
            }
        }
    }

    
    public void SetGlobalRtpc(AudioRtpc rtpc, float value)
    {
        EnsureInitialized();
        if (rtpc == null || !rtpc.IsValid)
            return;

        float v = rtpc.ClampValue(value);
        AkSoundEngine.SetRTPCValue(rtpc.rtpcName, v);
    }

    public void SetRtpcOn(AudioRtpc rtpc, float value, GameObject emitter)
    {
        EnsureInitialized();
        if (rtpc == null || !rtpc.IsValid || emitter == null)
            return;

        float v = rtpc.ClampValue(value);
        AkSoundEngine.SetRTPCValue(rtpc.rtpcName, v, emitter);
    }

    private void EnsureInitialized()
    {
        if (_globalEmitter == null)
            throw new InvalidOperationException("AudioStateModel is not initialized. Call Initialize(globalEmitter) from GameContext.");
    }
}
