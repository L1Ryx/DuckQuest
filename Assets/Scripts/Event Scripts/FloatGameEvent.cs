using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/Float Game Event", fileName = "FloatGameEvent_")]
public sealed class FloatGameEvent : ScriptableObject
{
    // Inspector listeners (same idea as your GameEvent)
    public event Action<float> OnRaised;

    // Runtime listeners (matching your current pattern)
    private readonly List<Action<float>> _runtimeListeners = new();

    public void Raise(float value)
    {
        OnRaised?.Invoke(value);

        // Iterate on a snapshot to avoid issues if listeners mutate list during callback
        for (int i = 0; i < _runtimeListeners.Count; i++)
        {
            _runtimeListeners[i]?.Invoke(value);
        }
    }

    public void RegisterRuntimeListener(Action<float> listener)
    {
        if (listener == null) return;
        if (!_runtimeListeners.Contains(listener))
            _runtimeListeners.Add(listener);
    }

    public void UnregisterRuntimeListener(Action<float> listener)
    {
        if (listener == null) return;
        _runtimeListeners.Remove(listener);
    }
}