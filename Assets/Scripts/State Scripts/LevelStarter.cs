using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelStarter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float startDelay = 1f;

    [SerializeField] private bool lockInteractionsAtStart = true;
    [Header("Events")]
    [SerializeField] private UnityEvent lockInteractions;
    
    private void Start()
    {
        if (!Game.IsReady || Game.Ctx.LevelState == null)
        {
            Debug.LogWarning("LevelStarter: GameContext not ready.");
            return;
        }

        StartCoroutine(DelayedStart(startDelay));

        if (lockInteractionsAtStart)
        {
            lockInteractions?.Invoke();
        }
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        Game.Ctx.LevelState.BeginLevel();
    }

    public void PrintStartLog()
    {
        Debug.Log("LevelStarter: Level has started.");
    }
}