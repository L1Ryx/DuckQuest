using UnityEngine;

public class PlayerProximityHighlighter : MonoBehaviour
{
    [Header("Match your PlayerInteractor settings")]
    [SerializeField] private float interactRadius = 1.25f;
    [SerializeField] private LayerMask interactableMask;

    [Header("Performance")]
    [SerializeField] private float refreshRateHz = 15f;

    private float timer;
    private IProximityHighlightable currentClosest;

    private void Update()
    {
        timer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, refreshRateHz);
        if (timer < interval) return;
        timer = 0f;

        UpdateClosest();
    }

    private void UpdateClosest()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableMask);

        IProximityHighlightable best = null;
        float bestDistSq = float.PositiveInfinity;
        Vector2 p = transform.position;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            // Only objects that are actually interactable should be highlight candidates.
            var interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            var highlightable = col.GetComponentInParent<IProximityHighlightable>();
            if (highlightable == null) continue;

            Vector2 closestPoint = col.ClosestPoint(p);
            float dSq = (closestPoint - p).sqrMagnitude;

            if (dSq < bestDistSq)
            {
                bestDistSq = dSq;
                best = highlightable;
            }
        }

        if (best == currentClosest) return;

        // Turn off old
        currentClosest?.SetIsClosest(false);

        // Turn on new
        currentClosest = best;
        currentClosest?.SetIsClosest(true);
    }

    private void OnDisable()
    {
        // Clean up prompt if player is disabled or scene changes
        currentClosest?.SetIsClosest(false);
        currentClosest = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
