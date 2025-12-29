using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIFloatyJuice : MonoBehaviour
{
    [Header("Float (position)")]
    [SerializeField] private float floatDistance = 6f;     // in UI pixels
    [SerializeField] private float floatDuration = 1.6f;
    [SerializeField] private Ease floatEase = Ease.InOutSine;

    [Header("Breath (scale)")]
    [SerializeField] private float scaleAmount = 0.02f;    // 0.02 = 2%
    [SerializeField] private float scaleDuration = 1.9f;
    [SerializeField] private Ease scaleEase = Ease.InOutSine;

    [Header("Variation")]
    [SerializeField] private bool randomizeDelay = true;
    [SerializeField] private float maxDelaySeconds = 0.35f;

    [SerializeField] private bool randomizePhase = true;
// 0..1 means "percent through the loop"
    [SerializeField, Range(0f, 1f)] private float maxPhaseNormalized = 1f;


    private RectTransform rt;
    private Vector2 basePos;
    private Vector3 baseScale;
    private Sequence seq;

    private void Awake()
    {
        rt = (RectTransform)transform;
        basePos = rt.anchoredPosition;
        baseScale = rt.localScale;
    }

    private void OnEnable()
    {
        // In case something else moved us while disabled
        basePos = rt.anchoredPosition;
        baseScale = rt.localScale;

        StartJuice();
    }

    private void OnDisable()
    {
        StopJuice();

        // Reset to avoid “stuck offset” if prefab is reused
        if (rt != null)
        {
            rt.anchoredPosition = basePos;
            rt.localScale = baseScale;
        }
    }

    private void StartJuice()
    {
        StopJuice();

        // Reset to baseline so loops are stable
        rt.anchoredPosition = basePos;
        rt.localScale = baseScale;

        float delay = 0f;
        if (randomizeDelay)
            delay = Random.Range(0f, Mathf.Max(0f, maxDelaySeconds));

        seq = DOTween.Sequence()
            .SetUpdate(true)
            .SetAutoKill(false);

        // Forward half-cycle
        seq.Join(rt.DOAnchorPosY(basePos.y + floatDistance, floatDuration).SetEase(floatEase));
        seq.Join(rt.DOScale(baseScale * (1f + scaleAmount), scaleDuration).SetEase(scaleEase));

        // Loop the entire sequence back and forth forever
        seq.SetLoops(-1, LoopType.Yoyo);

        // Apply delay (optional)
        if (delay > 0f)
            seq.SetDelay(delay);

        // Apply phase randomization (optional)
        // This jumps the tween to a random position within its duration,
        // so multiple instances won't align even if they start the same frame.
        if (randomizePhase)
        {
            float dur = seq.Duration(includeLoops: false);
            if (dur > 0f)
            {
                float phase01 = Random.Range(0f, Mathf.Clamp01(maxPhaseNormalized));
                float t = phase01 * dur;

                // Force setup then jump without triggering callbacks
                seq.ForceInit();
                seq.Goto(t, andPlay: true);
            }
        }
        else
        {
            // ensure it plays
            seq.Play();
        }
    }


    public void Rebase()
    {
        if (rt == null) rt = (RectTransform)transform;

        basePos = rt.anchoredPosition;
        baseScale = rt.localScale;

        if (isActiveAndEnabled)
            StartJuice();
    }
    

    private void StopJuice()
    {
        if (seq != null)
        {
            seq.Kill();
            seq = null;
        }

        // Also kill any leftover tweens on this target (defensive)
        if (rt != null)
            DOTween.Kill(rt);
    }
}
