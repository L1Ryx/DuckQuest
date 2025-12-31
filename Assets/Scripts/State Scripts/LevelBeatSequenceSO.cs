using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelBeatSequence", menuName = "Beats/LevelBeatSequence")]
public class LevelBeatSequenceSO : ScriptableObject
{
    [Header("Metadata")]
    [SerializeField] private string levelId;
    [TextArea] [SerializeField] private string description;

    [Header("Ordered Beats")]
    [SerializeField] private List<LevelBeatSO> beats = new();

    public string LevelId => levelId;
    public IReadOnlyList<LevelBeatSO> Beats => beats;
}