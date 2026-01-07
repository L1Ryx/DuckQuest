using UnityEngine;
using UnityEngine.Events;

public sealed class FloatGameEventListener : MonoBehaviour
{
    [SerializeField] private FloatGameEvent gameEvent;

    [SerializeField] private UnityEvent<float> response;

    private void OnEnable()
    {
        if (gameEvent != null)
            gameEvent.OnRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        if (gameEvent != null)
            gameEvent.OnRaised -= OnEventRaised;
    }

    private void OnEventRaised(float value)
    {
        response?.Invoke(value);
    }
}