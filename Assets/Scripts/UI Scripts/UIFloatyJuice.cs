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
    [SerializeField] private bool randomizePhase = true;

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

        float phase = 0f;
        if (randomizePhase)
            phase = Random.Range(0f, 0.35f);

        // Reset to baseline so loops are stable
        rt.anchoredPosition = basePos;
        rt.localScale = baseScale;

        seq = DOTween.Sequence()
            .SetUpdate(true)
            .SetAutoKill(false)
            .SetDelay(phase);

        // Forward "half-cycle" (base -> up / base -> bigger)
        seq.Join(rt.DOAnchorPosY(basePos.y + floatDistance, floatDuration).SetEase(floatEase));
        seq.Join(rt.DOScale(baseScale * (1f + scaleAmount), scaleDuration).SetEase(scaleEase));

        // Loop the entire sequence back and forth forever
        seq.SetLoops(-1, LoopType.Yoyo);
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
