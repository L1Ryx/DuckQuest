using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Sequentially reveals a set of TMP_Text lines with a typing effect, then fades them out together.
/// Attach to the title card panel parent (which should have a CanvasGroup).
/// </summary>
[DisallowMultipleComponent]
public class TitleCardSequencer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Tooltip("Optional explicit order. If empty, all TMP_Text children will be used in hierarchy order.")]
    [SerializeField] private List<TMP_Text> lines = new List<TMP_Text>();

    [Header("Timing")]
    [Tooltip("Seconds to wait after a line finishes typing before starting the next line.")]
    [Min(0f)][SerializeField] private float delayAfterLine = 0.35f;

    [Tooltip("Characters per second.")]
    [Min(0.1f)][SerializeField] private float typeSpeedCps = 35f;

    [Tooltip("Seconds to wait after the final line finishes typing before fading out.")]
    [Min(0f)][SerializeField] private float delayAfterLastLine = 0.75f;

    [Header("Fade Out")]
    [Tooltip("Fade-out speed in alpha per second. (e.g., 2 = fade out in ~0.5s)")]
    [Min(0.01f)][SerializeField] private float fadeOutRate = 2f;

    [Header("Options")]
    [Tooltip("Use unscaled time (ignores Time.timeScale), useful for title cards.")]
    [SerializeField] private bool useUnscaledTime = true;

    [Tooltip("If true, disables the panel GameObject at the end.")]
    [SerializeField] private bool disableOnFinish = true;

    private readonly Dictionary<TMP_Text, string> _fullTexts = new();
    private Coroutine _sequenceCo;

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        CacheLinesAndTexts();
        HardResetVisuals();
    }

    /// <summary>
    /// Starts the title card reveal + fade sequence.
    /// Safe to call multiple times; it will restart the sequence.
    /// </summary>
    public void StartSequence()
    {
        CacheLinesAndTexts();

        if (_sequenceCo != null)
            StopCoroutine(_sequenceCo);

        _sequenceCo = StartCoroutine(SequenceRoutine());
    }

    /// <summary>
    /// Immediately stops any running sequence and hides the title card.
    /// </summary>
    public void StopAndHide()
    {
        if (_sequenceCo != null)
        {
            StopCoroutine(_sequenceCo);
            _sequenceCo = null;
        }

        HardResetVisuals();

        if (disableOnFinish)
            gameObject.SetActive(false);
    }

    private void CacheLinesAndTexts()
    {
        if (lines == null)
            lines = new List<TMP_Text>();

        if (lines.Count == 0)
        {
            // Hierarchy order by default. Include inactive children so you can keep the panel disabled initially.
            var found = GetComponentsInChildren<TMP_Text>(includeInactive: true);
            lines.AddRange(found);
        }

        _fullTexts.Clear();
        foreach (var t in lines)
        {
            if (t == null) continue;
            _fullTexts[t] = t.text ?? string.Empty;
        }
    }

    private void HardResetVisuals()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // Ensure texts are ready for maxVisibleCharacters typing.
        foreach (var kvp in _fullTexts)
        {
            var t = kvp.Key;
            if (t == null) continue;

            t.text = kvp.Value;
            t.ForceMeshUpdate();
            t.maxVisibleCharacters = 0;
        }
    }

    private IEnumerator SequenceRoutine()
    {
        // Show panel
        if (disableOnFinish && !gameObject.activeSelf)
            gameObject.SetActive(true);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        // Type each line sequentially
        for (int i = 0; i < lines.Count; i++)
        {
            var t = lines[i];
            if (t == null) continue;

            // Restore full text and hide it initially
            t.text = _fullTexts.TryGetValue(t, out var full) ? full : (t.text ?? string.Empty);
            t.ForceMeshUpdate();
            t.maxVisibleCharacters = 0;

            int totalChars = t.textInfo.characterCount;

            // If TMP hasn't populated yet (rare), force again after a frame
            if (totalChars == 0 && t.text.Length > 0)
            {
                yield return null;
                t.ForceMeshUpdate();
                totalChars = t.textInfo.characterCount;
            }

            yield return TypeTextRoutine(t, totalChars);

            // Wait after this line (use last-line delay if last)
            if (i == lines.Count - 1)
                yield return StartCoroutine(WaitRoutine(delayAfterLastLine));
            else
                yield return StartCoroutine(WaitRoutine(delayAfterLine));
            
        }

        // Fade out everything at once (via CanvasGroup)
        yield return FadeOutRoutine();

        if (disableOnFinish)
            gameObject.SetActive(false);

        _sequenceCo = null;
    }

    private IEnumerator TypeTextRoutine(TMP_Text t, int totalChars)
    {
        if (totalChars <= 0)
        {
            // Empty line (or whitespace-only) — still allow an immediate "finish"
            t.maxVisibleCharacters = 0;
            yield break;
        }

        float secondsPerChar = 1f / Mathf.Max(0.1f, typeSpeedCps);
        int visible = 0;

        while (visible < totalChars)
        {
            visible++;
            t.maxVisibleCharacters = visible;
            yield return StartCoroutine(WaitRoutine(secondsPerChar));
        }
    }   

    private IEnumerator FadeOutRoutine()
    {
        if (canvasGroup == null)
            yield break;

        float alpha = canvasGroup.alpha;
        while (alpha > 0f)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            alpha -= fadeOutRate * dt;
            canvasGroup.alpha = Mathf.Max(0f, alpha);
            yield return null;
        }
    }

    private IEnumerator WaitRoutine(float seconds)
    {
        if (seconds <= 0f)
            yield break;

        if (useUnscaledTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
    }
}
