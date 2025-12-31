using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelBeat", menuName = "Beats/LevelBeat")]
public class LevelBeatSO : ScriptableObject
{
    [Header("Metadata")]
    [SerializeField] private string beatId;
    [TextArea] [SerializeField] private string description;

    [Header("Beat Actions")]
    [Tooltip("Raised immediately when this beat becomes active (in order).")]
    [SerializeField] private List<GameEvent> onEnterEvents = new();

    [Header("Beat Progression")]
    [Tooltip("When this event is raised, the director advances to the next beat.")]
    [SerializeField] private GameEvent advanceEvent;

    public string BeatId => beatId;
    public IReadOnlyList<GameEvent> OnEnterEvents => onEnterEvents;
    public GameEvent AdvanceEvent => advanceEvent;
}