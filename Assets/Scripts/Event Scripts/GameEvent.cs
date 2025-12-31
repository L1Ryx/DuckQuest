using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    [Header("Metadata")]
    [SerializeField] private string description;
    [SerializeField] private bool isImportant;

    private readonly List<GameEventListener> listeners = new();
    private event Action RuntimeListeners;

    public void Raise()
    {
        // 1) MonoBehaviour listeners (existing)
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();

        // 2) Code listeners (new)
        RuntimeListeners?.Invoke();
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }

    // New:
    public void RegisterRuntimeListener(Action callback) => RuntimeListeners += callback;
    public void UnregisterRuntimeListener(Action callback) => RuntimeListeners -= callback;
}