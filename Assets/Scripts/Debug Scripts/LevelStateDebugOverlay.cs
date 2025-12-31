using TMPro;
using UnityEngine;

public class LevelStateDebugOverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void Reset()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!Game.IsReady || Game.Ctx.LevelState == null || text == null)
            return;

        text.text = $"Level State Debug: {Game.Ctx.LevelState.CurrentState}";
    }
}