using UnityEngine;

public sealed class AudioEmitter : MonoBehaviour
{
    [Tooltip("If true, calls go through Game.Ctx.Audio using this object as the emitter.")]
    [SerializeField] private bool useGameContext = true;

    public uint Play(AudioCue cue)
    {
        if (cue == null) return 0;

        if (useGameContext && Game.IsReady)
            return Game.Ctx.Audio.PlayCueOn(cue, gameObject);

        // Fallback if context isn't ready (e.g., testing in isolation)
        if (!cue.HasPlayEvent) return 0;
        return AkSoundEngine.PostEvent(cue.playEvent, gameObject);
    }

    public void Stop(AudioCue cue)
    {
        if (cue == null) return;

        if (useGameContext && Game.IsReady)
        {
            Game.Ctx.Audio.StopCueOn(cue, gameObject);
            return;
        }

        if (!cue.HasStopEvent) return;
        AkSoundEngine.PostEvent(cue.stopEvent, gameObject);
    }

    public void SetRtpc(string rtpcName, float value)
    {
        if (string.IsNullOrWhiteSpace(rtpcName)) return;

        if (useGameContext && Game.IsReady)
        {
            Game.Ctx.Audio.SetRtpcOn(rtpcName, value, gameObject);
            return;
        }

        AkSoundEngine.SetRTPCValue(rtpcName, value, gameObject);
    }
}