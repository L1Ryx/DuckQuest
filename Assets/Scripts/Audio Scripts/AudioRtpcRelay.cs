using UnityEngine;

/// <summary>
/// Inspector-friendly relay: FloatGameEventListener.Response(float) -> set RTPC.
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
    [SerializeField] private string rtpcName;

    [Tooltip("If true, clamps incoming values to [minValue, maxValue].")]
    [SerializeField] private bool clamp = false;

    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 100f;

    [Tooltip("Optional multiplier applied to incoming value (e.g., normalize speed).")]
    [SerializeField] private float multiplier = 1f;

    public void SetRtpc(float value)
    {
        if (!Game.IsReady)
            return;

        if (string.IsNullOrWhiteSpace(rtpcName))
            return;

        float v = value * multiplier;
        if (clamp)
            v = Mathf.Clamp(v, minValue, maxValue);

        switch (routeMode)
        {
            case RouteMode.Global:
                Game.Ctx.Audio.SetGlobalRtpc(rtpcName, v);
                break;

            case RouteMode.SelfEmitter:
                Game.Ctx.Audio.SetRtpcOn(rtpcName, v, gameObject);
                break;

            case RouteMode.TargetEmitter:
                if (targetEmitter != null)
                    Game.Ctx.Audio.SetRtpcOn(rtpcName, v, targetEmitter);
                break;
        }
    }
}