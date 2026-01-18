using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Character")]
public sealed class DialogueCharacter : ScriptableObject
{
    [Header("Identity")]
    public string displayName;

    [Header("UI")]
    public Sprite portrait;

    [Header("Audio")]
    [Tooltip("Played for each character typed during dialogue (e.g. quack, chirp, piano tick).")]
    public AudioCue typingCue;

    [Tooltip("Optional cue played once when this character starts a line.")]
    public AudioCue lineStartCue;
}