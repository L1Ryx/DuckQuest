using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Cue", fileName = "AudioCue_")]
public sealed class AudioCue : ScriptableObject
{
    [Header("Wwise Events (Names)")]
    [Tooltip("Wwise Event name to post (e.g., Play_UI_Click, Play_Door_Open).")]
    public string playEvent;

    [Tooltip("Optional Wwise Event name used to stop/fade this cue (e.g., Stop_Music_MainTheme).")]
    public string stopEvent;

    [Header("RTPC to Apply (Optional)")]
    public RtpcBinding[] rtpcBindings;

    [Serializable]
    public struct RtpcBinding
    {
        [Tooltip("RTPC SO that's been set up in Unity.")]
        public AudioRtpc rtpc;

        [Tooltip("Value to set when this cue is played.")]
        public float value;

        [Tooltip("If true, applies globally. If false, applies to the emitter.")]
        public bool isGlobal;
    }

    public bool HasPlayEvent => !string.IsNullOrWhiteSpace(playEvent);
    public bool HasStopEvent => !string.IsNullOrWhiteSpace(stopEvent);
}