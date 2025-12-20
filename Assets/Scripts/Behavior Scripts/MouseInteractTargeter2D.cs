using UnityEngine;

public class MouseInteractTargeter2D : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactRadius = 1.25f;

    [Header("Debug")]
    [SerializeField] private bool drawDebug = false;

    private IInteractable currentTarget;
    private IProximityHighlightable currentHighlight;

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    private void Update()
    {
        UpdateHoverTarget();
    }

    public IInteractable CurrentTarget => currentTarget;

    private void UpdateHoverTarget()
    {
        if (worldCamera == null)
            return;

        // Convert mouse position to world
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld3 = worldCamera.ScreenToWorldPoint(mouseScreen);
        Vector2 mouseWorld = new Vector2(mouseWorld3.x, mouseWorld3.y);

        // Find collider under cursor
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, interactableMask);

        IInteractable newTarget = null;
        IProximityHighlightable newHighlight = null;

        if (hit != null)
        {
            // Must be interactable
            var candidate = hit.GetComponentInParent<IInteractable>();
            if (candidate != null)
            {
                // Proximity gate: must be within range of player
                Vector2 p = transform.position;
                Vector2 closest = hit.ClosestPoint(p);
                float distSq = (closest - p).sqrMagnitude;

                if (distSq <= interactRadius * interactRadius)
                {
                    newTarget = candidate;
                    newHighlight = hit.GetComponentInParent<IProximityHighlightable>();
                }
            }
        }

        if (newHighlight == currentHighlight && newTarget == currentTarget)
            return;

        // Turn off old prompt
        currentHighlight?.SetIsClosest(false);

        currentTarget = newTarget;
        currentHighlight = newHighlight;

        // Turn on new prompt (we reuse SetIsClosest as "highlight on")
        currentHighlight?.SetIsClosest(true);

        if (drawDebug)
            Debug.Log(currentTarget != null ? $"Hover target: {((MonoBehaviour)currentTarget).name}" : "Hover target: none");
    }

    private void OnDisable()
    {
        currentHighlight?.SetIsClosest(false);
        currentHighlight = null;
        currentTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
