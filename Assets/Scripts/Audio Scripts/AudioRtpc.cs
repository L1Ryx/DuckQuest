using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio RTPC", fileName = "AudioRtpc_")]
public sealed class AudioRtpc : ScriptableObject
{
    [Header("Wwise")]
    [Tooltip("Exact RTPC name in Wwise (case-sensitive).")]
    public string rtpcName;

    [Header("Optional Metadata")]
    [Tooltip("If enabled, values will be clamped to [minValue, maxValue] when using helpers that respect clamping.")]
    public bool clamp = false;

    public float minValue = 0f;
    public float maxValue = 100f;

    [Tooltip("Optional default value (for your own initialization/reset logic).")]
    public float defaultValue = 0f;

    public bool IsValid => !string.IsNullOrWhiteSpace(rtpcName);

    public float ClampValue(float value)
    {
        if (!clamp) return value;
        return Mathf.Clamp(value, minValue, maxValue);
    }
}