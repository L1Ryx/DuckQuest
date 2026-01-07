using UnityEngine;

/// <summary>
/// Inspector-friendly relay: FloatGameEventListener.Response(float) -> set RTPC (via AudioRtpc SO).
/// </summary>
public sealed class AudioRtpcRelay : MonoBehaviour
{
    public enum RouteMode
    {
        Global,
        SelfEmitter,
        TargetEmitter
    }

    [Header("Routing")]
    [SerializeField] private RouteMode routeMode = RouteMode.Global;

    [Tooltip("Used when RouteMode is TargetEmitter.")]
    [SerializeField] private GameObject targetEmitter;

    [Header("RTPC")]
    [SerializeField] private AudioRtpc rtpc;

    [Tooltip("Multiplier applied to incoming value (e.g., normalize meters/sec to 0-100).")]
    [SerializeField] private float multiplier = 1f;

    [Tooltip("Optional additive offset after multiplier.")]
    [SerializeField] private float offset = 0f;

    public void SetRtpc(float value)
    {
        if (!Game.IsReady)
            return;

        if (rtpc == null || !rtpc.IsValid)
            return;

        float v = (value * multiplier) + offset;

        switch (routeMode)
        {
            case RouteMode.Global:
                Game.Ctx.Audio.SetGlobalRtpc(rtpc, v);
                break;

            case RouteMode.SelfEmitter:
                Game.Ctx.Audio.SetRtpcOn(rtpc, v, gameObject);
                break;

            case RouteMode.TargetEmitter:
                if (targetEmitter != null)
                    Game.Ctx.Audio.SetRtpcOn(rtpc, v, targetEmitter);
                break;
        }
    }

    /// <summary>
    /// Convenience for parameterless events (until everything is float-driven):
    /// sets the RTPC to its defaultValue.
    /// </summary>
    public void SetDefault()
    {
        SetRtpc(rtpc != null ? rtpc.defaultValue : 0f);
    }
}