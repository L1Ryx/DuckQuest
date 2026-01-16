using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FragileBridge : InventoryCostInteractable
{
    [Header("Visuals")]
    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private Sprite repairedSprite;

    [Tooltip("Optional override. If null, will use SpriteRenderer on this GameObject.")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Colliders")]
    [Tooltip("The collider that blocks the player when the bridge is broken. Disabled when repaired.")]
    [SerializeField] private Collider2D blockingCollider;

    [Tooltip("A trigger collider that detects when the player steps onto/off the bridge. Should stay enabled.")]
    [SerializeField] private Collider2D playerDetectorTrigger;

    [Header("Rules")]
    [Tooltip("Tag used to detect the player stepping onto/off the bridge.")]
    [SerializeField] private string playerTag = "Player";
    
    [Header("Detector Query")]
    [SerializeField] private LayerMask playerLayerMask; // set this to Player layer in inspector

    [Header("Audio")] 
    [SerializeField] private AudioCue buildCue;

    [SerializeField] private AudioCue breakCue;

    private bool isRepaired;
    private bool playerWasOnBridge;
    private InventoryCostInteractableHoverPanel hoverPanel;

    private void Awake()
    {
        hoverPanel = GetComponent<InventoryCostInteractableHoverPanel>();
        
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        // Validate references (fail fast)
        if (blockingCollider == null)
            Debug.LogError($"{name}: Missing blockingCollider reference.", this);

        if (playerDetectorTrigger == null)
            Debug.LogError($"{name}: Missing playerDetectorTrigger reference.", this);
        else if (!playerDetectorTrigger.isTrigger)
            Debug.LogError($"{name}: playerDetectorTrigger must have isTrigger = true.", this);

        // Start broken by default (safe)
        SetBrokenState();
    }
    
    private void SyncPlayerAlreadyOnBridge()
    {
        if (playerDetectorTrigger == null) return;

        // Use the trigger's bounds to query overlaps.
        // This works even if the player was already inside at the moment of repair.
        var hits = new Collider2D[8];
        var filter = new ContactFilter2D
        {
            useLayerMask = playerLayerMask.value != 0,
            layerMask = playerLayerMask,
            useTriggers = true
        };

        int count = Physics2D.OverlapCollider(playerDetectorTrigger, filter, hits);

        for (int i = 0; i < count; i++)
        {
            if (hits[i] == null) continue;
            if (IsPlayerCollider(hits[i]))
            {
                playerWasOnBridge = true;
                return;
            }
        }
    }

    /// <summary>
    /// Prevent paying again while repaired.
    /// This works with the base-class gate + hover-panel active/inactive logic.
    /// </summary>
    protected override bool CanInteractNow(GameObject interactor)
    {
        return !isRepaired;
    }

    protected override void OnPaymentSucceeded(GameObject interactor)
    {
        SetRepairedState();
    }

    private void SetRepairedState()
    {
        isRepaired = true;
        playerWasOnBridge = false;

        Game.Ctx.Audio.PlayCueGlobal(buildCue);
        hoverPanel?.ForceHide();

        if (targetRenderer != null && repairedSprite != null)
            targetRenderer.sprite = repairedSprite;

        if (blockingCollider != null)
            blockingCollider.enabled = false;

        // Keep playerDetectorTrigger enabled so we can detect exit.
        if (playerDetectorTrigger != null)
            playerDetectorTrigger.enabled = true;
        
        SyncPlayerAlreadyOnBridge();
    }

    private void SetBrokenState()
    {
        isRepaired = false;
        playerWasOnBridge = false;
        
        hoverPanel?.Refresh();

        if (targetRenderer != null && brokenSprite != null)
            targetRenderer.sprite = brokenSprite;

        if (blockingCollider != null)
            blockingCollider.enabled = true;

        if (playerDetectorTrigger != null)
            playerDetectorTrigger.enabled = true;

        // IMPORTANT: allow repeat cycle.
        // We do not want base "hasBeenUsed" to permanently block interaction for this repeatable.
        hasBeenUsed = false;
    }

    public void OnDetectorEnter(Collider2D other)
    {
        if (!isRepaired) return;
        if (!IsPlayerCollider(other)) return;

        playerWasOnBridge = true;
    }

    public void OnDetectorExit(Collider2D other)
    {
        if (!isRepaired) return;
        if (!IsPlayerCollider(other)) return;

        if (!playerWasOnBridge) return;
        
        Game.Ctx.Audio.PlayCueGlobal(breakCue);
        SetBrokenState();
    }

    private bool IsPlayerCollider(Collider2D other)
    {
        if (other == null) return false;

        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.CompareTag(playerTag);

        var t = other.transform;
        while (t != null)
        {
            if (t.CompareTag(playerTag)) return true;
            t = t.parent;
        }
        return false;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        // Helpful editor-time guardrails
        if (playerDetectorTrigger != null && !playerDetectorTrigger.isTrigger)
            playerDetectorTrigger.isTrigger = true;
    }
#endif
}
