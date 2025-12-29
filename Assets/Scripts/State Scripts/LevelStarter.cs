using System.Collections;
using TMPro;
using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    [SerializeField] private float startDelay = 1f;
    private void Start()
    {
        if (!Game.IsReady || Game.Ctx.LevelState == null)
        {
            Debug.LogWarning("LevelStarter: GameContext not ready.");
            return;
        }

        StartCoroutine(DelayedStart(startDelay));
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